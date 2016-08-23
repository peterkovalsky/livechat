using Kookaburra.Domain.Model;
using Kookaburra.Domain.Repository;
using System.Linq;

namespace Kookaburra.Repository
{
    public class VisitorRepository : IVisitorRepository
    {
        private KookaburraContext _context;

        public VisitorRepository(KookaburraContext context)
        {
            _context = context;
        }

        public Visitor AddVisitor(Visitor visitor)
        {
            _context.Visitors.Add(visitor);
            _context.SaveChanges(); 

            return visitor;
        }

        public Visitor CheckForVisitor(string name, string email, string sessionId)
        {
            var existingVisitor = _context.Visitors
                .Where(v => (v.Name == name && v.Email == email) || (v.SessionId == sessionId))
                .SingleOrDefault();

            return existingVisitor;
        }
    }
}