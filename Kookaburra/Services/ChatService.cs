using Kookaburra.Domain.Repository;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Web;

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

        public void ConnectVisitor(string name, string email, string location, string connectionId)
        {
            _visitorRepository.CheckForVisitor()
        }
    }
}