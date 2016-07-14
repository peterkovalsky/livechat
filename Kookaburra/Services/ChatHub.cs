using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Kookaburra.Domain.Repository;
using Microsoft.AspNet.Identity;
using Kookaburra.Common;

namespace Kookaburra.Services
{
    public class ChatHub : Hub
    {
        private readonly IOperatorRepository _operatorRepository;

        public ChatHub(IOperatorRepository operatorRepository)
        {
            _operatorRepository = operatorRepository;
        }


        [Authorize]
        public void ConnectOperator()
        {
            var operatorObj = _operatorRepository.Get(Context.User.Identity.GetUserId());

            ChatOperation.ConnectOperator(operatorObj.Account.Identifier, operatorObj.Id, Context.ConnectionId, operatorObj.FirstName);
        }

        public void SendToOperator(string name, string message, string operatorId)
        {
            Clients.Clients(new List<string>() { Context.ConnectionId, operatorId })
                .addNewMessageToPage(name, message, DateTime.UtcNow.JsDateTime(), "client", Context.ConnectionId);
        }

        public void SendToVisitor(string message, string visitorId)
        {
            Clients.Clients(new List<string>() { Context.ConnectionId, visitorId })
                .addNewMessageToPage("REMOVE", message, DateTime.UtcNow.JsDateTime(), "operator", visitorId);
        }

        public string GetOperatorId(string companyId)
        {
            return Guid.NewGuid().ToString();
        }

      

        /// <summary>
        /// A visitor connects
        /// </summary>        
        public string ConnectClient(string clientName, string companyId, string currentPage)
        {
            var location = "Sydney, Australia";

            var operatorId = ChatOperation.GetFirstAvailableOperator(companyId);
            if (!string.IsNullOrEmpty(operatorId))
            {
                ChatOperation.ConnectVisitor(Context.ConnectionId, operatorId, clientName);

                Clients.Clients(new List<string>() { operatorId })
                    .clientConnected(Context.ConnectionId, clientName, DateTime.UtcNow.JsDateTime(), location, currentPage);
            }

            return operatorId;
        }

        public void DisconnectClient(string clientId)
        {
            Clients.Clients(new List<string>() { clientId }).orderToDisconnect();
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
            var operatorId = ChatOperation.GetOperatorConnectionId(Context.ConnectionId);

            // Operator was disconnected
            if (!string.IsNullOrEmpty(operatorId))
            {
                var clientName = ChatOperation.GetClientName(Context.ConnectionId);
                Clients.Clients(new List<string>() { operatorId })
                    .clientDisconnected(Context.ConnectionId, clientName, DateTime.UtcNow.JsDateTime());
            }

            ChatOperation.Disconnect(Context.ConnectionId);

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
    }
}