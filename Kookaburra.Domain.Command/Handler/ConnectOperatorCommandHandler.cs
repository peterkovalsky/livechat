using Kookaburra.Domain.Command.Model;
using Kookaburra.Repository;
using System.Linq;

namespace Kookaburra.Domain.Command.Handler
{
    public class ConnectOperatorCommandHandler : ICommandHandler<ConnectOperatorCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public ConnectOperatorCommandHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }


        public void Execute(ConnectOperatorCommand command)
        {
            var operatorEntity = _context.Operators.Where(o => o.Identity == command.OperatorKey).SingleOrDefault();

            _chatSession.AddOperator(operatorEntity.Id, operatorEntity.FirstName, operatorEntity.Account.Identifier, command.OperatorConnectionId);
        }
    }
}