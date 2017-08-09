using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.MarkMessageAsRead
{
    public class MarkMessageAsReadCommandHandler : ICommandHandler<MarkMessageAsReadCommand>
    {
        private readonly KookaburraContext _context;

        public MarkMessageAsReadCommandHandler(KookaburraContext context)
        {
            _context = context;         
        }

        public async Task ExecuteAsync(MarkMessageAsReadCommand command)
        {
            var message = await _context.OfflineMessages.Where(om => 
                                                               om.Id == command.MessageId 
                                                            && om.Account.Identifier == command.AccountKey)
                                                        .SingleOrDefaultAsync();

            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}