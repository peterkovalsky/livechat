using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Kookaburra.Services
{
    public class ChatHub : Hub
    {
        public void SendToOperator(string name, string message, string operatorId)
        {
            Clients.Clients(new List<string>() { Context.ConnectionId, operatorId })
                .addNewMessageToPage(name, message, DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds, "client", Context.ConnectionId);
        }

        public void SendToVisitor(string name, string message, string clientId)
        {
            Clients.Clients(new List<string>() { Context.ConnectionId, clientId })
                .addNewMessageToPage(name, message, DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds, "operator", clientId);
        }

        public string GetOperatorId(string companyId)
        {
            return Guid.NewGuid().ToString();
        }

        public void ConnectOperator(string operatorName, string companyId)
        {
            ChatOperation.ConnectOperator(companyId, Context.ConnectionId, operatorName);
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
                ChatOperation.ConnectClient(Context.ConnectionId, operatorId, clientName);

                Clients.Clients(new List<string>() { operatorId })
                    .clientConnected(Context.ConnectionId, clientName, DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds, location, currentPage);
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
            var operatorId = ChatOperation.GetOperatorId(Context.ConnectionId);

            // Operator was disconnected
            if (!string.IsNullOrEmpty(operatorId))
            {
                var clientName = ChatOperation.GetClientName(Context.ConnectionId);
                Clients.Clients(new List<string>() { operatorId })
                    .clientDisconnected(Context.ConnectionId, clientName, DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
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