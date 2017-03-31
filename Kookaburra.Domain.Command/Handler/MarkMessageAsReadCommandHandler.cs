using Kookaburra.Domain.Command.Model;
using Kookaburra.Repository;
using System.Linq;

namespace Kookaburra.Domain.Command.Handler
{
    public class MarkMessageAsReadCommandHandler : ICommandHandler<MarkMessageAsReadCommand>
    {
        private readonly KookaburraContext _context;

        public MarkMessageAsReadCommandHandler(KookaburraContext context)
        {
            _context = context;         
        }

        public void Execute(MarkMessageAsReadCommand command)
        {
            var message = _context.OfflineMessages.Where(om => 
                                                         om.Id == command.MessageId 
                                                      && om.Account.Operators.Any(o => o.Identity == command.OperatorIdentity))
                                                  .SingleOrDefault();

            if (message != null)
            {
                message.IsRead = true;
                _context.SaveChanges();
            }
        }
    }
}