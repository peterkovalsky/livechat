using Hangfire;
using Kookaburra.Common;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
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

        private readonly IVisitorChatService _visitorChatService;

        public BackgroundJobs(IQueryHandler<TimmedOutConversationsQuery, Task<TimmedOutConversationsQueryResult>> timmedOutConversationsQueryHandler,               
            IVisitorChatService visitorChatService)
        {
            _timmedOutConversationsQueryHandler = timmedOutConversationsQueryHandler;                  
            _visitorChatService = visitorChatService;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task TimeoutInactiveConversations()
        {
            var result = await _timmedOutConversationsQueryHandler.ExecuteAsync(new TimmedOutConversationsQuery(30));            

            foreach (var conversation in result.Conversations)
            {
                var currentSession = _visitorChatService.GetCurrentSessionByIdentity(conversation.VisitorSessionId);
                
                await _visitorChatService.StopChatAsync(conversation.VisitorSessionId, conversation.ConversationId);

                DisconnectVisitor(currentSession, UserType.System);              
            }
        }

        private void DisconnectVisitor(CurrentSessionResponse currentSession, UserType disconnectedBy)
        {
            if (currentSession != null)
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

                var diconnectView = new DisconnectViewModel
                {
                    VisitorSessionId = currentSession.VisitorIdentity,
                    TimeStamp = DateTime.UtcNow.JsDateTime(),
                    DisconnectedBy = disconnectedBy.ToString()
                };

                // Notify all operator instances
                hubContext.Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(currentSession.VisitorIdentity);
                hubContext.Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnected(diconnectView);
                // Notify all visitor instances
                hubContext.Clients.Clients(currentSession.VisitorConnectionIds).visitorDisconnected(diconnectView);
            }
        }
    }
}