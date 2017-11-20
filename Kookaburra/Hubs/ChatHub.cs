using AutoMapper;
using Kookaburra.Common;
using Kookaburra.Domain.Common;
using Kookaburra.Models;
using Kookaburra.Models.Chat;
using Kookaburra.Models.Home;
using Kookaburra.Services.Chats;
using Kookaburra.Services.OfflineMessages;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services
{
    public class ChatHub : Hub
    {
        private readonly IOfflineMessageService _offlineMessageService;
        private readonly IOperatorChatService _operatorChatService;
        private readonly IVisitorChatService _visitorChatService;

        public const string COOKIE_SESSION_ID = "kookaburra.visitor.sessionid";      
       

        public ChatHub(IOfflineMessageService offlineMessageService, IOperatorChatService operatorChatService, IVisitorChatService visitorChatService)
        {
            _offlineMessageService = offlineMessageService;
            _operatorChatService = operatorChatService;
            _visitorChatService = visitorChatService;
        }

        
        [Authorize]
        public async Task<OperatorCurrentChatsViewModel> ConnectOperator()
        {           
            await _operatorChatService.ConnectOperatorAsync(Context.User.Identity.GetUserId(), Context.ConnectionId);            
            
            var queryResult = await _operatorChatService.GetCurrentChatsAsync(Context.User.Identity.GetUserId());

            return Mapper.Map<OperatorCurrentChatsViewModel>(queryResult);
        }

        /// <summary>
        /// Message from OPERATOR to VISITOR
        /// </summary>
        [Authorize]
        public async Task<dynamic> SendToVisitor(string operatorName, string message, string visitorSessionId, long messageId)
        {
            var dateSent = DateTime.UtcNow;       
            var currentSession = _visitorChatService.GetCurrentSessionByIdentity(visitorSessionId);

            var messageView = new MessageViewModel
            {
                Author = operatorName,
                Text = message,
                SentBy = UserType.Operator,
                SentOn = dateSent
            };

            // Notify all operator instances (mutiple tabs)            
            Clients.Clients(currentSession.OperatorConnectionIds.AllBut(Context.ConnectionId)).sendMessageToOperator(messageView, visitorSessionId);
            // Notify all visitor instances (mutiple tabs)
            Clients.Clients(currentSession.VisitorConnectionIds.AllBut(Context.ConnectionId)).sendMessageToVisitor(messageView);

            await _operatorChatService.OperatorMessagedAsync(visitorSessionId, message, dateSent);            

            return new
            {
                visitorSessionId = visitorSessionId,
                messageId = messageId
            };
        }

        [Authorize]
        public async Task<CurrentConversationsViewModel> ResumeOperatorChat()
        {
            var chats = await _operatorChatService.ResumeOperatorAsync(Context.User.Identity.GetUserId());         

            return new CurrentConversationsViewModel
            {
                Conversations = Mapper.Map<List<OperatorConversationViewModel>>(chats)
            };
        }

        /// <summary>
        ///  Operator wants to stop conversation with visitor
        /// </summary> 
        [Authorize]
        public async Task FinishChattingWithVisitor(string visitorSessionId)
        {     
            var currentSession = _visitorChatService.GetCurrentSessionByIdentity(visitorSessionId);

            await _visitorChatService.StopChatAsync(visitorSessionId);            

            DisconnectVisitor(currentSession, UserType.Operator);          
        }

        [Authorize]
        public async Task<DashboardViewModel> LoadDashboard()
        {
            var currentChats = await _operatorChatService.GetLiveChatsAsync(Context.User.Identity.GetUserId());
            var newMessages = await _offlineMessageService.TotalNewOfflineMessagesAsync(Context.User.Identity.GetUserId());

            return new DashboardViewModel
            {
                NewOfflineMessages = newMessages,
                TotalCurrentChats = currentChats.Count,
                CurrentChats = currentChats.Select(c => Mapper.Map<LiveChatViewModel>(c)).ToList()
            };
        }

        public override Task OnConnected()
        {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            //_chatService.DisconnectOperatorAsync(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }

        #region Helpers
        private void DisconnectVisitor(CurrentSessionResponse currentSession, UserType disconnectedBy)
        {
            if (currentSession != null)
            {
                var diconnectView = new DisconnectViewModel
                {
                    VisitorSessionId = currentSession.VisitorIdentity,
                    TimeStamp = DateTime.UtcNow.JsDateTime(),
                    DisconnectedBy = disconnectedBy.ToString()
                };

                // Notify all operator instances
                Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(currentSession.VisitorIdentity);
                Clients.Clients(currentSession.OperatorConnectionIds.AllBut(Context.ConnectionId)).visitorDisconnected(diconnectView);
                // Notify all visitor instances
                Clients.Clients(currentSession.VisitorConnectionIds.AllBut(Context.ConnectionId)).visitorDisconnected(diconnectView);
            }
        }     
        #endregion
    }
}