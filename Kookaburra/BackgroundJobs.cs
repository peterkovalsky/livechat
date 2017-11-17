using Hangfire;
using Kookaburra.Common;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.CurrentSession;
using Kookaburra.Domain.Query.TimmedOutConversations;
using Kookaburra.Models;
using Kookaburra.Services;
using Kookaburra.Services.Chats;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace Kookaburra
{
    public class BackgroundJobs
    {        
        private readonly IQueryHandler<TimmedOutConversationsQuery, Task<TimmedOutConversationsQueryResult>> _timmedOutConversationsQueryHandler;
        private readonly IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> _currentSessionQueryHandler;

        private readonly IChatService _chatService;

        public BackgroundJobs(IQueryHandler<TimmedOutConversationsQuery, Task<TimmedOutConversationsQueryResult>> timmedOutConversationsQueryHandler,     
            IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> currentSessionQueryHandler,
            IChatService chatService)
        {
            _timmedOutConversationsQueryHandler = timmedOutConversationsQueryHandler;            
            _currentSessionQueryHandler = currentSessionQueryHandler;
            _chatService = chatService;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task TimeoutInactiveConversations()
        {
            var result = await _timmedOutConversationsQueryHandler.ExecuteAsync(new TimmedOutConversationsQuery(30));            

            foreach (var conversation in result.Conversations)
            {            
                var query = new CurrentSessionQuery()
                {
                    VisitorSessionId = conversation.VisitorSessionId
                };
                var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);
                
                await _chatService.StopChatAsync(conversation.VisitorSessionId, conversation.ConversationId);

                DisconnectVisitor(currentSession, UserType.System);              
            }
        }

        private void DisconnectVisitor(CurrentSessionQueryResult currentSession, UserType disconnectedBy)
        {
            if (currentSession != null)
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

                var diconnectView = new DisconnectViewModel
                {
                    VisitorSessionId = currentSession.VisitorSessionId,
                    TimeStamp = DateTime.UtcNow.JsDateTime(),
                    DisconnectedBy = disconnectedBy.ToString()
                };

                // Notify all operator instances
                hubContext.Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(currentSession.VisitorSessionId);
                hubContext.Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnected(diconnectView);
                // Notify all visitor instances
                hubContext.Clients.Clients(currentSession.VisitorConnectionIds).visitorDisconnected(diconnectView);
            }
        }
    }
}