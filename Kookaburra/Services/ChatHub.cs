using AutoMapper;
using Kookaburra.Common;
using Kookaburra.Domain.AvailableOperator;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.ConnectOperator;
using Kookaburra.Domain.Command.OperatorMessaged;
using Kookaburra.Domain.Command.StartVisitorChat;
using Kookaburra.Domain.Command.StopConversation;
using Kookaburra.Domain.Command.VisitorMessaged;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.CurrentChats;
using Kookaburra.Domain.Query.CurrentSession;
using Kookaburra.Domain.Query.ResumeOperator;
using Kookaburra.Domain.ResumeVisitorChat;
using Kookaburra.Models;
using Kookaburra.Models.Chat;
using Kookaburra.Models.Widget;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Kookaburra.Services
{
    public class ChatHub : Hub
    {
        private readonly ICommandHandler<ConnectOperatorCommand> _connectOperatorCommandHandler;
        private readonly ICommandHandler<OperatorMessagedCommand> _operatorMessagedCommandHandler;
        private readonly ICommandHandler<StopConversationCommand> _stopConversationCommandHandler;
        private readonly ICommandHandler<StartVisitorChatCommand> _startVisitorChatCommandHandler;
        private readonly ICommandHandler<VisitorMessagedCommand> _visitorMessagedCommandHandler;

        private readonly IQueryHandler<CurrentChatsQuery, Task<CurrentChatsQueryResult>> _currentChatsQueryHandler;
        private readonly IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> _currentSessionQueryHandler;
        private readonly IQueryHandler<ResumeOperatorQuery, Task<ResumeOperatorQueryResult>> _resumeOperatorQueryHandler;
        private readonly IQueryHandler<AvailableOperatorQuery, Task<AvailableOperatorQueryResult>> _availableOperatorQueryHandler;
        private readonly IQueryHandler<ResumeVisitorChatQuery, Task<ResumeVisitorChatQueryResult>> _resumeVisitorChatQueryHandler;
        
        public const string COOKIE_SESSION_ID = "kookaburra.visitor.sessionid";

        public ChatHub(ICommandHandler<ConnectOperatorCommand> connectOperatorCommandHandler,
            ICommandHandler<OperatorMessagedCommand> operatorMessagedCommandHandler,
            ICommandHandler<StopConversationCommand> stopConversationCommandHandler,
            ICommandHandler<StartVisitorChatCommand> startVisitorChatCommandHandler,
            ICommandHandler<VisitorMessagedCommand> visitorMessagedCommandHandler,
            IQueryHandler<CurrentChatsQuery, Task<CurrentChatsQueryResult>> currentChatsQueryHandler,
            IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> currentSessionQueryHandler,
            IQueryHandler<ResumeOperatorQuery, Task<ResumeOperatorQueryResult>> resumeOperatorQueryHandler,
            IQueryHandler<AvailableOperatorQuery, Task<AvailableOperatorQueryResult>> availableOperatorQueryHandler,
            IQueryHandler<ResumeVisitorChatQuery, Task<ResumeVisitorChatQueryResult>> resumeVisitorChatQueryHandler
           )
        {
            _connectOperatorCommandHandler = connectOperatorCommandHandler;
            _operatorMessagedCommandHandler = operatorMessagedCommandHandler;
            _stopConversationCommandHandler = stopConversationCommandHandler;
            _startVisitorChatCommandHandler = startVisitorChatCommandHandler;
            _visitorMessagedCommandHandler = visitorMessagedCommandHandler;

            _currentChatsQueryHandler = currentChatsQueryHandler;
            _currentSessionQueryHandler = currentSessionQueryHandler;
            _resumeOperatorQueryHandler = resumeOperatorQueryHandler;
            _availableOperatorQueryHandler = availableOperatorQueryHandler;
            _resumeVisitorChatQueryHandler = resumeVisitorChatQueryHandler;
        }


        #region Operator Calls
        [Authorize]
        public async Task<OperatorCurrentChatsViewModel> ConnectOperator()
        {
            await _connectOperatorCommandHandler.ExecuteAsync(new ConnectOperatorCommand(Context.ConnectionId, Context.User.Identity.GetUserId()));

            var queryResult = await _currentChatsQueryHandler.ExecuteAsync(new CurrentChatsQuery(Context.User.Identity.GetUserId()));

            return Mapper.Map<OperatorCurrentChatsViewModel>(queryResult);
        }

        /// <summary>
        /// Message from OPERATOR to VISITOR
        /// </summary>
        [Authorize]
        public async Task SendToVisitor(string operatorName, string message, string visitorSessionId)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
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
            Clients.Clients(currentSession.OperatorConnectionIds).sendMessageToOperator(messageView, visitorSessionId);
            // Notify all visitor instances (mutiple tabs)
            Clients.Clients(currentSession.VisitorConnectionIds).sendMessageToVisitor(messageView);

            await _operatorMessagedCommandHandler.ExecuteAsync(new OperatorMessagedCommand(visitorSessionId, message, dateSent, Context.User.Identity.GetUserId()));
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
            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

            await _stopConversationCommandHandler.ExecuteAsync(new StopConversationCommand(visitorSessionId, Context.User.Identity.GetUserId()));

            var diconnectView = new DisconnectViewModel
            {
                VisitorSessionId = visitorSessionId,
                TimeStamp = DateTime.UtcNow.JsDateTime()
            };

            // Notify all operator instances
            Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(visitorSessionId);
            Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedByOperator(diconnectView);
            // Notify all visitor instances
            Clients.Clients(currentSession.VisitorConnectionIds).visitorDisconnectedByOperator(diconnectView);
        }
        #endregion

        #region Visitor Calls

        public async Task<InitWidgetViewModel> InitWidget(string accountKey)
        {
            var query = new AvailableOperatorQuery(accountKey);
            var operatorResult = await _availableOperatorQueryHandler.ExecuteAsync(query);

            if (operatorResult != null) // Is there any available operator
            {
                var sessionId = GetSessionId();
                if (sessionId != null) // check if it's a returning visitor
                {
                    var queryResume = new ResumeVisitorChatQuery(sessionId, Context.ConnectionId);
                    var resumedConversation = await _resumeVisitorChatQueryHandler.ExecuteAsync(queryResume);

                    if (resumedConversation != null) // check if session is still alive
                    {                    
                        // Online - resume chat
                        return new InitWidgetViewModel
                        {
                            Step = WidgetStepType.Resume.ToString(),
                            ResumedChat = Mapper.Map<ConversationViewModel>(resumedConversation)
                        };
                    }
                }              

                // Introduction                 
                return new InitWidgetViewModel
                {
                    Step = WidgetStepType.Introduction.ToString()
                };
            }

            // Offline - no operator available
            return new InitWidgetViewModel
            {
                Step = WidgetStepType.Offline.ToString()
            };
        }

        public async Task<StartChatViewModel> StartChat(IntroductionViewModel visitor)
        {
            var query = new AvailableOperatorQuery(visitor.AccountKey);
            var availableOperator = await _availableOperatorQueryHandler.ExecuteAsync(query);

            // if operator is available - establish connection
            if (availableOperator != null)
            {
                var newSessionId = Guid.NewGuid().ToString();

                var command = new StartVisitorChatCommand(availableOperator.OperatorId, visitor.Name, newSessionId, Context.User.Identity.GetUserId())
                {
                    Page = visitor.PageUrl,
                    VisitorIP = WebHelper.GetIPAddress(),
                    VisitorEmail = visitor.Email
                };
                await _startVisitorChatCommandHandler.ExecuteAsync(command);


                var queryResume = new ResumeVisitorChatQuery(newSessionId, Context.ConnectionId);
                var resumedConversation = await _resumeVisitorChatQueryHandler.ExecuteAsync(queryResume);

                if (resumedConversation != null)
                {
                    var visitorInfo = new OperatorConversationViewModel
                    {
                        SessionId = newSessionId,
                        VisitorName = visitor.Name,
                        Email = visitor.Email,
                        Country = resumedConversation.VisitorInfo.Country,
                        City = resumedConversation.VisitorInfo.City, 
                        Region = resumedConversation.VisitorInfo.Region,
                        CurrentUrl = visitor.PageUrl,
                        StartTime = DateTime.UtcNow.JsDateTime()
                    };

                    // Notify all operator instances about this visitor
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnectedGlobal(newSessionId);
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnected(visitorInfo);

                    return new StartChatViewModel
                    {
                        SessionId = newSessionId,
                        OperatorName = availableOperator.OperatorName
                    };
                }
            }

            return null;
        }

        public void SendOfflineMessage(OfflineViewModel offlineMessage)
        {

        }

        /// <summary>
        /// Message from VISITOR to OPERATOR
        /// </summary>
        public async Task SendToOperator(string message)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
            {
                VisitorConnectionId = Context.ConnectionId
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

            var messageView = new MessageViewModel
            {
                Author = currentSession.VisitorName,
                Text = message,
                SentBy = UserType.Visitor,
                SentOn = dateSent
            };

            // Notify all visitor instances (mutiple tabs)
            Clients.Clients(currentSession.VisitorConnectionIds).sendMessageToVisitor(messageView);
            // Notify all operator instances (mutiple tabs)   
            Clients.Clients(currentSession.OperatorConnectionIds).sendMessageToOperator(messageView, currentSession.VisitorSessionId);

            await _visitorMessagedCommandHandler.ExecuteAsync(new VisitorMessagedCommand(Context.ConnectionId, message, dateSent, Context.User.Identity.GetUserId()));
        }

        /// <summary>
        /// Visitor wants to stop the conversation with operator
        /// </summary>
        public async Task FinishChattingWithOperator()
        {
            var visitorSessionId = GetSessionId();

            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

            await _stopConversationCommandHandler.ExecuteAsync(new StopConversationCommand(visitorSessionId, Context.User.Identity.GetUserId()));

            var diconnectView = new DisconnectViewModel
            {
                VisitorSessionId = visitorSessionId,
                TimeStamp = DateTime.UtcNow.JsDateTime()
            };
            // Notify all operator instances
            Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(visitorSessionId);
            Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedByVisitor(diconnectView);

            // Notify all visitor instances       
            Clients.Clients(currentSession.VisitorConnectionIds).visitorDisconnectedByVisitor();
        }

        public async Task StopConversation(string visitorSessionId)
        {
            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

            if (currentSession != null)
            {
                var diconnectView = new DisconnectViewModel
                {
                    VisitorSessionId = visitorSessionId,
                    TimeStamp = DateTime.UtcNow.JsDateTime()
                };
                // Notify all operator instances
                Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(visitorSessionId);
                Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedByVisitor(diconnectView);

                // Notify all visitor instances       
                Clients.Clients(currentSession.VisitorConnectionIds).visitorDisconnectedByVisitor();
            }
        }
        #endregion

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
        private string GetSessionId()
        {
            var httpContext = Context.Request.GetHttpContext();

            var sessionId = httpContext.Request.Cookies[COOKIE_SESSION_ID];

            if (sessionId == null || string.IsNullOrWhiteSpace(sessionId.Value))
            {
                return null;
            }

            return sessionId.Value;
        }

        private void SetSessionId(string newSessionId)
        {
            var httpContext = Context.Request.GetHttpContext();
            httpContext.Response.Cookies.Set(new HttpCookie(COOKIE_SESSION_ID, newSessionId));
        }
        #endregion
    }
}