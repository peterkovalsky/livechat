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
        public void SendToOperator(string name, string message)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery
            {
                VisitorConnectionId = Context.ConnectionId
            };
            var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);          
            
            Clients.Clients(new List<string>() { Context.ConnectionId, currentSession.OperatorConnectionId })
                .sendMessageToOperator(name, message, dateSent.JsDateTime(), currentSession.VisitorSessionId);
            
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

        public ConversationViewModel CheckVisitorSession(string sessionId)
        {
            var query = new ContinueConversationQuery(sessionId, Context.ConnectionId);
            var resumedConversation = _queryDispatcher.Execute<ContinueConversationQuery, ContinueConversationQueryResult>(query);

            if (resumedConversation != null)
            {
                return Mapper.Map<ConversationViewModel>(resumedConversation);            
            }

            return null;
        }

        public string ConnectVisitor(string name, string email, string page, string accountKey)
        {
            //http://freegeoip.net/json/rio-matras.com
            string location = "Sydney, Australia";


            var operatorResult = _queryDispatcher.Execute<AvailableOperatorQuery, AvailableOperatorQueryResult>(new AvailableOperatorQuery(accountKey));

            // if operator is available - establish connection
            if (operatorResult != null)
            {
                var sessionId = Guid.NewGuid().ToString();

                Clients.Clients(new List<string>() { operatorResult.OperatorConnectionId })
                    .clientConnected(sessionId, name, DateTime.UtcNow.JsDateTime(), location, page);

             

                return sessionId;
            }

            return null;
        }

        public void DisconnectVisitor(string visitorConnectionId)
        {
            _commandDispatcher.Execute(new StopConversationCommand(visitorConnectionId));

            Clients.Clients(new List<string>() { visitorConnectionId }).orderToDisconnect();
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
    }
}