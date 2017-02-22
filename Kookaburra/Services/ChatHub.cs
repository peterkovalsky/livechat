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

            var query = new CurrentSessionQuery
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);

            // Notify all visitor instances 
            Clients.Clients(currentSession.VisitorConnectionIds)
                .sendMessageToVisitor(operatorName, message, dateSent.JsDateTime());

            _commandDispatcher.Execute(new OperatorMessagedCommand(visitorSessionId, message, dateSent));
        }

        [Authorize]
        public CurrentConversationsViewModel ResumeOperatorChat()
        {
            var query = new ResumeOperatorQuery(Context.User.Identity.GetUserId());
            var queryResult = _queryDispatcher.Execute<ResumeOperatorQuery, ResumeOperatorQueryResult>(query);

            return Mapper.Map<CurrentConversationsViewModel>(queryResult);
        }
        #endregion

        #region Visitor Calls
        /// <summary>
        /// Message from VISITOR to OPERATOR
        /// </summary>
        public void SendToOperator(string message)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery
            {
                VisitorConnectionId = Context.ConnectionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);
          
            var messageViewModel = new MessageViewModel
            {
                Author = currentSession.VisitorName,
                Text = message,
                Time = dateSent.JsDateTime(),
                SentBy = UserType.Visitor.ToString()
            };
            Clients.Clients(currentSession.OperatorConnectionIds)
                .sendMessageToOperator(messageViewModel, currentSession.VisitorSessionId);
            
            _commandDispatcher.Execute(new VisitorMessagedCommand(Context.ConnectionId, message, dateSent));            
        }

    

        public ConversationViewModel ConnectVisitor()
        {
            var httpContext = Context.Request.GetHttpContext();
            var sessionId = httpContext.Request.Cookies[COOKIE_SESSION_ID];

            if (sessionId == null || string.IsNullOrWhiteSpace(sessionId.Value))
            {
                return null;
            }

            var query = new ContinueConversationQuery(sessionId.Value, Context.ConnectionId);
            var resumedConversation = _queryDispatcher.Execute<ContinueConversationQuery, ContinueConversationQueryResult>(query);

            if (resumedConversation != null)
            {
                if (resumedConversation.IsNewConversation)
                {                    
                    var visitorInfo = new VisitorInfoViewModel
                    {
                        SessionId = sessionId.Value,
                        Name = resumedConversation.VisitorInfo.Name,
                        Location = resumedConversation.VisitorInfo.Location,
                        CurrentUrl = resumedConversation.VisitorInfo.CurrentUrl,
                        StartTime = DateTime.UtcNow.JsDateTime()
                    };

                    // Notify all operator instances about this visitor
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnectedGlobal(visitorInfo.SessionId);
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnected(visitorInfo);
                }

                var viewModel = Mapper.Map<ConversationViewModel>(resumedConversation);
                viewModel.VisitorName = resumedConversation.VisitorInfo.Name;
                viewModel.OperatorName = resumedConversation.OperatorInfo.Name;                           

                return viewModel;
            }                      

            return null;
        }

        public void DisconnectVisitor(string visitorSessionId)
        {
            _commandDispatcher.Execute(new StopConversationCommand(visitorSessionId));

            //Clients.Clients(new List<string>() { visitorConnectionId }).orderToDisconnect();
        }    

        public void VisitorClosesChat()
        {
            var visitorSessionId = GetVisitorSessionId();

            var query = new CurrentSessionQuery
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);

            _commandDispatcher.Execute(new StopConversationCommand(visitorSessionId));
          
            // Notify all operator instances
            Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(visitorSessionId);
            Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnected(visitorSessionId);
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