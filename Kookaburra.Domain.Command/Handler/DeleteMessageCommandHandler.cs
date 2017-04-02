using Kookaburra.Domain.Command.Model;
using Kookaburra.Repository;
using System.Linq;

namespace Kookaburra.Domain.Command.Handler
{
    public class DeleteMessageCommandHandler : ICommandHandler<DeleteMessageCommand>
    {
        private readonly KookaburraContext _context;
  

        public DeleteMessageCommandHandler(KookaburraContext context)
        {
            _context = context;            
        }

        public void Execute(DeleteMessageCommand command)
        {
            var message = _context.OfflineMessages.Where(om =>
                                                         om.Id == command.MessageId
                                                      && om.Account.Operators.Any(o => o.Identity == command.OperatorIdentity))
                                                  .SingleOrDefault();

            if (message != null)
            {                
                _context.OfflineMessages.Remove(message);
                _context.SaveChanges();
            }
        }
    }
}