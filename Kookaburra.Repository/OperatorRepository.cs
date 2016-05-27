using Kookaburra.Domain.Model;
using Kookaburra.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<Operator> GetList(string accountKey)
        {
            return _context.Operators.Where(o => o.Account.Identifier == accountKey).ToList();
        }

        public Account GetAccount(string operatorIdentity)
        {
            return _context.Accounts.Where(a => a.Operators.Any(o => o.Identity == operatorIdentity)).SingleOrDefault();
        }

        public void RecordOperatorActivity(string operatorIdentity)
        {
            var oper = _context.Operators.SingleOrDefault(o => o.Identity == operatorIdentity);
            if (oper != null)
            {
                oper.LastActivity = DateTime.UtcNow;

                _context.SaveChanges();
            }
        }

        public void ResetOperatorActivity(string operatorIdentity)
        {
            var oper = _context.Operators.SingleOrDefault(o => o.Identity == operatorIdentity);
            if (oper != null)
            {
                oper.LastActivity = null;

                _context.SaveChanges();
            }
        }

        public DateTime? GetLastActivity(string operatorIdentity)
        {
            return _context.Operators.Where(o => o.Identity == operatorIdentity).Select(o => o.LastActivity).SingleOrDefault();
        }
    }
}