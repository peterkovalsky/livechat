using Kookaburra.Domain.Repository;
using System;
using System.Linq;
using Kookaburra.Domain.Model;

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