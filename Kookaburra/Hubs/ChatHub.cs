using AutoMapper;
using Kookaburra.Common;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Account;
using Kookaburra.Domain.Query.CurrentChats;
using Kookaburra.Domain.Query.CurrentSession;
using Kookaburra.Domain.Query.ResumeOperator;
using Kookaburra.Models;
using Kookaburra.Models.Chat;
using Kookaburra.Models.Home;
using Kookaburra.Services.Chats;
using Kookaburra.Services.OfflineMessages;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services
{
    public class ChatHub : Hub
    {
        private readonly IQueryHandler<CurrentChatsQuery, Task<CurrentChatsQueryResult>> _currentChatsQueryHandler;
        private readonly IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> _currentSessionQueryHandler;
        private readonly IQueryHandler<ResumeOperatorQuery, Task<ResumeOperatorQueryResult>> _resumeOperatorQueryHandler;
        private readonly IQueryHandler<AccountQuery, Task<AccountQueryResult>> _accountQueryHandler;

        private readonly IChatService _chatService;
        private readonly IOfflineMessageService _offlineMessageService;

        public const string COOKIE_SESSION_ID = "kookaburra.visitor.sessionid";      
       

        public ChatHub(                                     
            IQueryHandler<CurrentChatsQuery, Task<CurrentChatsQueryResult>> currentChatsQueryHandler,
            IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> currentSessionQueryHandler,
            IQueryHandler<ResumeOperatorQuery, Task<ResumeOperatorQueryResult>> resumeOperatorQueryHandler,
            IQueryHandler<AccountQuery, Task<AccountQueryResult>> accountQueryHandler,
            IChatService chatService,
            IOfflineMessageService offlineMessageService)
        {
            _currentChatsQueryHandler = currentChatsQueryHandler;
            _currentSessionQueryHandler = currentSessionQueryHandler;
            _resumeOperatorQueryHandler = resumeOperatorQueryHandler;
            _accountQueryHandler = accountQueryHandler;

            _chatService = chatService;
            _offlineMessageService = offlineMessageService;
        }

        
        [Authorize]
        public async Task<OperatorCurrentChatsViewModel> ConnectOperator()
        {
            var accountResult = await _accountQueryHandler.ExecuteAsync(new AccountQuery(Context.User.Identity.GetUserId()));

            await _chatService.ConnectOperatorAsync(accountResult.AccountKey, Context.User.Identity.GetUserId(), Context.ConnectionId);            

            var queryResult = await _currentChatsQueryHandler.ExecuteAsync(new CurrentChatsQuery(Context.User.Identity.GetUserId(), accountResult.AccountKey));

            return Mapper.Map<OperatorCurrentChatsViewModel>(queryResult);
        }

        /// <summary>
        /// Message from OPERATOR to VISITOR
        /// </summary>
        [Authorize]
        public async Task<dynamic> SendToVisitor(string operatorName, string message, string visitorSessionId, long messageId)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

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

            await _chatService.OperatorMessagedAsync(visitorSessionId, message, dateSent);            

            return new
            {
                visitorSessionId = visitorSessionId,
                messageId = messageId
            };
        }

        [Authorize]
        public async Task<CurrentConversationsViewModel> ResumeOperatorChat()
        {
            var query = new ResumeOperatorQuery(Context.User.Identity.GetUserId());
            var queryResult = await _resumeOperatorQueryHandler.ExecuteAsync(query);

            return Mapper.Map<CurrentConversationsViewModel>(queryResult);
        }

        /// <summary>
        ///  Operator wants to stop conversation with visitor
        /// </summary> 
        [Authorize]
        public async Task FinishChattingWithVisitor(string visitorSessionId)
        {
            var query = new CurrentSessionQuery()
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

            await _chatService.StopChatAsync(visitorSessionId);            

            DisconnectVisitor(currentSession, UserType.Operator);          
        }

        [Authorize]
        public async Task<DashboardViewModel> LoadDashboard()
        {
            var currentChats = await _chatService.GetLiveChatsAsync(Context.User.Identity.GetUserId());
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
        private void DisconnectVisitor(CurrentSessionQueryResult currentSession, UserType disconnectedBy)
        {
            if (currentSession != null)
            {
                var diconnectView = new DisconnectViewModel
                {
                    VisitorSessionId = currentSession.VisitorSessionId,
                    TimeStamp = DateTime.UtcNow.JsDateTime(),
                    DisconnectedBy = disconnectedBy.ToString()
                };

                // Notify all operator instances
                Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(currentSession.VisitorSessionId);
                Clients.Clients(currentSession.OperatorConnectionIds.AllBut(Context.ConnectionId)).visitorDisconnected(diconnectView);
                // Notify all visitor instances
                Clients.Clients(currentSession.VisitorConnectionIds.AllBut(Context.ConnectionId)).visitorDisconnected(diconnectView);
            }
        }     
        #endregion
    }
}