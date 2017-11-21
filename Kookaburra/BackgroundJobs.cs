using Hangfire;
using Kookaburra.Common;
using Kookaburra.Domain.Common;
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
        private readonly IVisitorChatService _visitorChatService;
        private readonly IChatService _chatService;

        public BackgroundJobs(IVisitorChatService visitorChatService, IChatService chatService)
        {            
            _visitorChatService = visitorChatService;
            _chatService = chatService;
        }


        [AutomaticRetry(Attempts = 0)]
        public async Task TimeoutInactiveConversations()
        {
            var conversations = await _chatService.TimmedOutConversationsAsync(30);

            foreach (var conversation in conversations)
            {
                var currentSession = _visitorChatService.GetCurrentSessionByIdentity(conversation.VisitorIdentity);
                
                await _visitorChatService.StopChatAsync(conversation.VisitorIdentity, conversation.ConversationId);

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