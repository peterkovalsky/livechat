using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.DeleteMessage
{
    public class DeleteMessageCommandHandler : ICommandHandler<DeleteMessageCommand>
    {
        private readonly KookaburraContext _context;
  

        public DeleteMessageCommandHandler(KookaburraContext context)
        {
            _context = context;            
        }

        public async Task ExecuteAsync(DeleteMessageCommand command)
        {
            var message = await _context.OfflineMessages.Where(om =>
                                                               om.Id == command.MessageId
                                                            && om.Account.Identifier == command.AccountKey)
                                                        .SingleOrDefaultAsync();

            if (message != null)
            {                
                _context.OfflineMessages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}