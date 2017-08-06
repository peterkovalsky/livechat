using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.ReturningVisitor
{
    public class ReturningVisitorQueryHandler : IQueryHandler<ReturningVisitorQuery, Task<ReturningVisitorQueryResult>>
    {
        private readonly ChatSession _chatSession;
        private readonly KookaburraContext _context;

        public ReturningVisitorQueryHandler(ChatSession chatSession, KookaburraContext context)
        {
            _chatSession = chatSession;
            _context = context;
        }


        public async Task<ReturningVisitorQueryResult> ExecuteAsync(ReturningVisitorQuery query)
        {
            var chatSession = _chatSession.GetVisitorByVisitorSessionId(query.VisitorId);
            if (chatSession != null)
            {
                var visitor = await _context.Visitors.Where(v => v.SessionId == query.VisitorId && v.Account.Identifier == query.AccountKey).SingleOrDefaultAsync();
                if (visitor != null)
                {
                    return new ReturningVisitorQueryResult
                    {
                        VisitorName = visitor.Name,
                        VisitorEmail = visitor.Email
                    };
                }                
            }

            return null;
        }
    }
}