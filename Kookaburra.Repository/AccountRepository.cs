using Kookaburra.Domain.Model;
using Kookaburra.Domain.Repository;
using System.Linq;

namespace Kookaburra.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private KookaburraContext _context;

        public AccountRepository(KookaburraContext context)
        {
            _context = context;
        }      

        public Account Get(string identifier)
        {
            return _context.Accounts.Where(a => a.Identifier == identifier).SingleOrDefault();
        }
    }
} 