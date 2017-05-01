using AutoMapper;
using Kookaburra.Common;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models;
using Kookaburra.Models.Chat;
using Kookaburra.Models.Widget;
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
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public const string COOKIE_SESSION_ID = "kookaburra.visitor.sessionid";

        public ChatHub(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        #region Operator Calls
        [Authorize]
        public OperatorCurrentChatsViewModel ConnectOperator()
        {
            _commandDispatcher.Execute(new ConnectOperatorCommand(Context.ConnectionId, Context.User.Identity.GetUserId()));

            var queryResult = _queryDispatcher.Execute<CurrentChatsQuery, CurrentChatsQueryResult>(new CurrentChatsQuery(Context.User.Identity.GetUserId()));

            return Mapper.Map<OperatorCurrentChatsViewModel>(queryResult);
        }

        /// <summary>
        /// Message from OPERATOR to VISITOR
        /// </summary>
        [Authorize]
        public void SendToVisitor(string operatorName, string message, string visitorSessionId)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);
            
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

            _commandDispatcher.Execute(new OperatorMessagedCommand(visitorSessionId, message, dateSent, Context.User.Identity.GetUserId()));
        }

        [Authorize]
        public CurrentConversationsViewModel ResumeOperatorChat()
        {
            var query = new ResumeOperatorQuery(Context.User.Identity.GetUserId());
            var queryResult = _queryDispatcher.Execute<ResumeOperatorQuery, ResumeOperatorQueryResult>(query);

            return Mapper.Map<CurrentConversationsViewModel>(queryResult);
        }

        /// <summary>
        ///  Operator wants to stop conversation with visitor
        /// </summary> 
        [Authorize]
        public void FinishChattingWithVisitor(string visitorSessionId)
        {
            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);

            _commandDispatcher.Execute(new StopConversationCommand(visitorSessionId, Context.User.Identity.GetUserId()));

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

        public InitWidgetViewModel InitWidget(string accountKey)
        {
            var query = new AvailableOperatorQuery(accountKey, Context.User.Identity.GetUserId());
            var operatorResult = _queryDispatcher.Execute<AvailableOperatorQuery, AvailableOperatorQueryResult>(query);

            var httpContext = Context.Request.GetHttpContext();
            var sessionId = httpContext.Request.Cookies[COOKIE_SESSION_ID];

            if (operatorResult != null) // Is there any available operator
            {
                if (sessionId != null && !string.IsNullOrWhiteSpace(sessionId.Value)) // check if it's a returning visitor
                {
                    var sessionQuery = new CurrentSessionQuery(Context.User.Identity.GetUserId()) { VisitorSessionId = sessionId.Value };
                    var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(sessionQuery);

                    if (currentSession != null) // check if the session still alive 
                    {
                        var queryResume = new ContinueConversationQuery(sessionId.Value, Context.ConnectionId, Context.User.Identity.GetUserId());
                        var resumedConversation = _queryDispatcher.Execute<ContinueConversationQuery, ContinueConversationQueryResult>(queryResume);

                        if (resumedConversation != null)
                        {
                            if (resumedConversation.IsNewConversation)
                            {
                                var visitorInfo = new OperatorConversationViewModel
                                {
                                    SessionId = sessionId.Value,
                                    VisitorName = resumedConversation.VisitorInfo.Name,
                                    Location = @"{resumedConversation.VisitorInfo.Country}, {resumedConversation.VisitorInfo.City}",
                                    CurrentUrl = resumedConversation.VisitorInfo.CurrentUrl,
                                    StartTime = DateTime.UtcNow.JsDateTime()
                                };

                                // Notify all operator instances about this visitor
                                Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnectedGlobal(visitorInfo.SessionId);
                                Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnected(visitorInfo);
                            }

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

        public ConversationViewModel ConnectVisitor()
        {
            var httpContext = Context.Request.GetHttpContext();
            var sessionId = httpContext.Request.Cookies[COOKIE_SESSION_ID];

            if (sessionId == null || string.IsNullOrWhiteSpace(sessionId.Value))
            {
                return null;
            }

            var query = new ContinueConversationQuery(sessionId.Value, Context.ConnectionId, Context.User.Identity.GetUserId());
            var resumedConversation = _queryDispatcher.Execute<ContinueConversationQuery, ContinueConversationQueryResult>(query);

            if (resumedConversation != null)
            {
                if (resumedConversation.IsNewConversation)
                {
                    var visitorInfo = new OperatorConversationViewModel
                    {
                        SessionId = sessionId.Value,
                        VisitorName = resumedConversation.VisitorInfo.Name,
                        Location = @"{resumedConversation.VisitorInfo.Country}, {resumedConversation.VisitorInfo.City}",
                        CurrentUrl = resumedConversation.VisitorInfo.CurrentUrl,
                        StartTime = DateTime.UtcNow.JsDateTime()
                    };

                    // Notify all operator instances about this visitor
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnectedGlobal(visitorInfo.SessionId);
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnected(visitorInfo);
                }

                return Mapper.Map<ConversationViewModel>(resumedConversation);
            }

            return null;
        }

        /// <summary>
        /// Message from VISITOR to OPERATOR
        /// </summary>
        public void SendToOperator(string message)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
            {
                VisitorConnectionId = Context.ConnectionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);
          
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
            
            _commandDispatcher.Execute(new VisitorMessagedCommand(Context.ConnectionId, message, dateSent, Context.User.Identity.GetUserId()));            
        }        

        /// <summary>
        /// Visitor wants to stop the conversation with operator
        /// </summary>
        public void FinishChattingWithOperator()
        {
            var visitorSessionId = GetVisitorSessionId();

            var query = new CurrentSessionQuery(Context.User.Identity.GetUserId())
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);

            _commandDispatcher.Execute(new StopConversationCommand(visitorSessionId, Context.User.Identity.GetUserId()));

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

        private string GetVisitorSessionId()
        {
            var httpContext = Context.Request.GetHttpContext();

            var sessionId = httpContext.Request.Cookies[COOKIE_SESSION_ID];

            if (sessionId == null || string.IsNullOrWhiteSpace(sessionId.Value))
            {
                return null;
            }

            return sessionId.Value;
        }
    }
}