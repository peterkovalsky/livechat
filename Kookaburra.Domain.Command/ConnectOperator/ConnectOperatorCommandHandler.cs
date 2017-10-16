using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.ConnectOperator
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


        public async Task ExecuteAsync(ConnectOperatorCommand command)
        {
            var operatorEntity = await _context.Operators
                .Include(i => i.Account)
                .Where(o => o.Identifier == command.OperatorIdentity && o.Account.Identifier == command.AccountKey)
                .SingleOrDefaultAsync();

            _chatSession.AddOrUpdateOperator(operatorEntity.Id, operatorEntity.Identifier, operatorEntity.FirstName, operatorEntity.Account.Identifier, command.OperatorConnectionId);
        }
    }
}