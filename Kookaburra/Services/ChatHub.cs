using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Kookaburra.Domain.Repository;
using Microsoft.AspNet.Identity;
using Kookaburra.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;

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
            _chatService.ConnectOperator(Context.ConnectionId, Context.User.Identity.GetUserId());
        }

        public void SendToOperator(string name, string message, string operatorId)
        {
            var sentDate = DateTime.UtcNow;

            Clients.Clients(new List<string>() { Context.ConnectionId, operatorId })
                .sendMessageToOperator(name, message, sentDate.JsDateTime(), Context.ConnectionId);

            _chatService.LogMessage(Context.ConnectionId, operatorId, UserType.Visitor, message, sentDate);
        }


        public void SendToVisitor(string operatorName, string message, string visitorId)
        {
            var sentDate = DateTime.UtcNow;

            Clients.Clients(new List<string>() { Context.ConnectionId, visitorId })
                .sendMessageToVisitor(operatorName, message, sentDate.JsDateTime());

            _chatService.LogMessage(visitorId, Context.ConnectionId, UserType.Operator, message, sentDate);
        }


        public string ConnectVisitor(string name, string email, string page, string accountKey, string sessionId)
        {
            //http://freegeoip.net/json/rio-matras.com
            string location = "Sydney, Australia";


            var operatorResult = _queryDispatcher.Execute<AvailableOperatorQuery, AvailableOperatorQueryResult>(new AvailableOperatorQuery(accountKey));

            // if operator is available - establish connection
            if (operatorResult != null)
            {
                Clients.Clients(new List<string>() { operatorResult.OperatorConnectionId })
                    .clientConnected(Context.ConnectionId, name, DateTime.UtcNow.JsDateTime(), location, page);

                var command = new StartConversationCommand(Context.ConnectionId, name, sessionId, accountKey);
                command.Page = page;
                command.Location = location;
                command.VisitorEmail = email;

                _commandDispatcher.Execute(command);

                return operatorResult.OperatorConnectionId;
            }

            return null;
        }

        public void DisconnectVisitor(string visitorConnectionId)
        {
            _chatService.DisconnectVisitor(visitorConnectionId);

            Clients.Clients(new List<string>() { visitorConnectionId }).orderToDisconnect();
        }

        //public void DisconnectClient(string clientId)
        //{
        //    Clients.Clients(new List<string>() { clientId }).orderToDisconnect();
        //}

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