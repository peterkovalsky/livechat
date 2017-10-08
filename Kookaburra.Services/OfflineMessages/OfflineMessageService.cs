using Kookaburra.Repository;
using Kookaburra.Services.Accounts;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services.OfflineMessages
{
    public class OfflineMessageService : IOfflineMessageService
    {        
        private readonly KookaburraContext _context;
        private readonly IAccountService _accountService;

        public OfflineMessageService(KookaburraContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public async Task<int> TotalNewOfflineMessagesAsync(string operatorIdentity)
        {
            var account = await _accountService.GetAccountAsync(operatorIdentity);

            return await _context.OfflineMessages.Where(om =>
                                                        om.Visitor.Account.Identifier == account.Identifier
                                                    && !om.IsRead)
                                                 .CountAsync();
        }
    }
}