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

        public ChatHub(IOperatorRepository operatorRepository, IMessageRepository messageRepository, ChatService chatService)
        {
            _operatorRepository = operatorRepository;
            _messageRepository = messageRepository;
            _chatService = chatService;
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

            _chatService.
            
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


        public string ConnectVisitor(string name, string email, string page, string accountKey)
        {
            //http://freegeoip.net/json/rio-matras.com
            //string name, string email, string location, string sessionId, string connectionId, string accountKey

            string location = "Sydney, Australia";
            var availableOperatorId = _chatService.ConnectVisitor(name, email, location, Guid.NewGuid().ToString(), Context.ConnectionId, page, accountKey);

            if (!string.IsNullOrEmpty(availableOperatorId))
            {
                Clients.Clients(new List<string>() { availableOperatorId })
                    .clientConnected(Context.ConnectionId, name, DateTime.UtcNow.JsDateTime(), location, page);
            }

            return availableOperatorId;
        }

        /// <summary>
        /// A visitor connects
        /// </summary>        
        //public string ConnectClient(string clientName, string companyId, string currentPage)
        //{
        //    var location = "Sydney, Australia";

        //    var operatorId = ChatOperation.GetFirstAvailableOperator(companyId);
        //    if (!string.IsNullOrEmpty(operatorId))
        //    {
        //        ChatOperation.ConnectVisitor(Context.ConnectionId, operatorId, clientName);

        //        Clients.Clients(new List<string>() { operatorId })
        //            .clientConnected(Context.ConnectionId, clientName, DateTime.UtcNow.JsDateTime(), location, currentPage);
        //    }

        //    return operatorId;
        //}

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