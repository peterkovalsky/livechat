using Kookaburra.Domain.Repository;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Web;
using Kookaburra.Domain.Model;

namespace Kookaburra.Services
{
    public class ChatService
    {
        private readonly IOperatorRepository _operatorRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IVisitorRepository _visitorRepository;
        private readonly ChatSession _currentSession;


        public ChatService(ChatSession currentSession, IOperatorRepository operatorRepository, IMessageRepository messageRepository, IVisitorRepository visitorRepository)
        {
            _currentSession = currentSession;
            _operatorRepository = operatorRepository;
            _messageRepository = messageRepository;
            _visitorRepository = visitorRepository;
        }


        public void ConnectOperator(string connectionId, string operatorKey)
        {
            var operatorEntity = _operatorRepository.Get(operatorKey);

            _currentSession.AddOperator(operatorEntity.Id, operatorEntity.FirstName, operatorEntity.Account.Identifier, connectionId);
        }

        public string ConnectVisitor(string name, string email, string location, string sessionId, string connectionId, string page, string accountKey)
        {
            // record new/returning visitor
            var returningVisitor = _visitorRepository.CheckForVisitor(name, email, sessionId);
            if (returningVisitor == null)
            {
                returningVisitor = _visitorRepository.AddVisitor(new Visitor
                {
                    Name = name,
                    Email = email,
                    Location = location,
                    SessionId = sessionId,
                    Page = page
                });
            }

            // get available operator
            var operatorConnectionId = _currentSession.GetFirstAvailableOperator(accountKey);
            if (operatorConnectionId != null)
            {
                // add visitor to session
                _currentSession.AddVisitor(operatorConnectionId, connectionId, returningVisitor.Id, returningVisitor.Name);
            }

            return operatorConnectionId;
        }

        public void DisconnectVisitor(string connectionId)
        {
            _currentSession.RemoveVisitor(connectionId);
        }

        public void DisconnectOperator(string connectionId)
        {
            _currentSession.RemoveOperator(connectionId);
        }
    }
}