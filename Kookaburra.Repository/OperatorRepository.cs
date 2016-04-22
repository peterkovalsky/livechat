using Kookaburra.Domain.Repository;
using System.Linq;
using Kookaburra.Domain.Model;

namespace Kookaburra.Repository
{
    public class OperatorRepository : IOperatorRepository
    {
        private KookaburraContext _context;

        public OperatorRepository(KookaburraContext context)
        {
            _context = context;
        }

        public Operator Get(string identity)
        {
            return _context.Operators.Where(o => o.Identity == identity).SingleOrDefault();
        }

        public Account GetAccount(string operatorIdentity)
        {
            return _context.Accounts.Where(a => a.Operators.Any(o => o.Identity == operatorIdentity)).SingleOrDefault();
        }
    }
}