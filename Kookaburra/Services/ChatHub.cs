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

namespace Kookaburra.Services
{
    public class ChatHub : Hub
    {
        private readonly IOperatorRepository _operatorRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ChatService _chatService;

        public ChatHub(IOperatorRepository operatorRepository, IMessageRepository messageRepository)
        {
            _operatorRepository = operatorRepository;
            _messageRepository = messageRepository;
        } 


        [Authorize]
        public void ConnectOperator()
        {
            _chatService.ConnectOperator(Context.ConnectionId, Context.User.Identity.GetUserId());
        }

        public void SendToOperator(string name, string text, string operatorId)
        {
            var sentDate = DateTime.UtcNow;

            Clients.Clients(new List<string>() { Context.ConnectionId, operatorId })
                .sendMessageToOperator(name, text, sentDate.JsDateTime(), Context.ConnectionId);
           
            _messageRepository.AddMessage(
                new Message
                {
                    OperatorId = 0,
                    VisitorId = 0,
                    Text = text,
                    DateSent = sentDate
                });
        }

        public void SendToVisitor(string operatorName, string message, string visitorId)
        {
            Clients.Clients(new List<string>() { Context.ConnectionId, visitorId })
                .sendMessageToVisitor(operatorName, message, DateTime.UtcNow.JsDateTime());
        }

        public string GetOperatorId(string companyId)
        {
            return Guid.NewGuid().ToString();
        }

      
        public string ConnectVisitor(string name, )
        {
            //http://freegeoip.net/json/rio-matras.com

            _chatService.ConnectVisitor()
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