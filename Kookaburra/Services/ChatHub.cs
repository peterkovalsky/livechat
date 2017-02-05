using AutoMapper;
using Kookaburra.Common;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
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


        [Authorize]
        public void ConnectOperator()
        {
            _commandDispatcher.Execute(new ConnectOperatorCommand(Context.ConnectionId, Context.User.Identity.GetUserId()));            
        }

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
            
            Clients.Clients(new List<string>() { currentSession.OperatorConnectionId })
                .sendMessageToOperator(currentSession.VisitorName, message, dateSent.JsDateTime(), currentSession.VisitorSessionId);
            
            _commandDispatcher.Execute(new VisitorMessagedCommand(Context.ConnectionId, message, dateSent));            
        }

        /// <summary>
        /// Message from OPERATOR to VISITOR
        /// </summary>
        public void SendToVisitor(string operatorName, string message, string visitorSessionId)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery
            {
                VisitorSessionId = visitorSessionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);

            Clients.Clients(new List<string>() { Context.ConnectionId, currentSession.VisitorConnectionId })
                .sendMessageToVisitor(operatorName, message, dateSent.JsDateTime());

            _commandDispatcher.Execute(new OperatorMessagedCommand(visitorSessionId, message, dateSent));
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
                    // Notify operator about this visitor
                    Clients.Clients(new List<string>() { resumedConversation.OperatorInfo.ConnectionId })
                        .clientConnected(sessionId.Value, resumedConversation.VisitorInfo.Name, DateTime.UtcNow.JsDateTime(), resumedConversation.VisitorInfo.Location, resumedConversation.VisitorInfo.Page);
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

            _commandDispatcher.Execute(new StopConversationCommand(visitorSessionId));
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

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    var operatorId = ChatOperation.GetOperatorConnectionId(Context.ConnectionId);

        //    // Operator was disconnected
        //    if (!string.IsNullOrEmpty(operatorId))
        //    {
        //        var clientName = ChatOperation.GetClientName(Context.ConnectionId);
        //        Clients.Clients(new List<string>() { operatorId })
        //            .clientDisconnected(Context.ConnectionId, clientName, DateTime.UtcNow.JsDateTime());
        //    }

        //    ChatOperation.Disconnect(Context.ConnectionId);

        //    return base.OnDisconnected(stopCalled);
        //}

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