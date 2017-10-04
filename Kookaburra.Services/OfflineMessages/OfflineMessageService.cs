using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services.OfflineMessages
{
    public class OfflineMessageService : IOfflineMessageService
    {
        private readonly KookaburraContext _context;

        public OfflineMessageService(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<int> TotalNewOfflineMessagesAsync(string operatorIdentity)
        {
            var account = await _context.Accounts.Where(a => a.Operators.Any(o => o.Identity == operatorIdentity)).SingleOrDefaultAsync();

            return await _context.OfflineMessages.Where(om =>
                                                        om.Visitor.Account.Identifier == account.Identifier
                                                    && !om.IsRead)
                                                 .CountAsync();
        }
    }
}