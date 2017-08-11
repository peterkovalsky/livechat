using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.Account
{
    public class AccountQueryHandler : IQueryHandler<AccountQuery, Task<AccountQueryResult>>
    {
        private readonly KookaburraContext _context;

        public AccountQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<AccountQueryResult> ExecuteAsync(AccountQuery query)
        {
            var account = await _context.Accounts.Where(a => a.Operators.Any(o => o.Identity == query.OperatorIdentity)).SingleOrDefaultAsync();

            return new AccountQueryResult
            {
                AccountKey = account.Identifier
            };
        }
    }
}