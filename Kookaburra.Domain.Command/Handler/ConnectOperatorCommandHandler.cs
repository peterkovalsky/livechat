using Kookaburra.Domain.Command.Model;
using Kookaburra.Repository;
using System.Linq;
using System.Data.Entity;

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
            var operatorEntity = _context.Operators.Include(i => i.Account).Where(o => o.Identity == command.OperatorIdentity).SingleOrDefault();

            _chatSession.AddOrUpdateOperator(operatorEntity.Id, operatorEntity.Identity, operatorEntity.FirstName, operatorEntity.Account.Identifier, command.OperatorConnectionId);
        }
    }
}