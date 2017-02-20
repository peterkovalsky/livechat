using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.Handler
{
    public class ResumeOperatorQueryHandler : IQueryHandler<ResumeOperatorQuery, ResumeOperatorQueryResult>
    {
        private readonly KookaburraContext _context;

        public ResumeOperatorQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public ResumeOperatorQueryResult Execute(ResumeOperatorQuery query)
        {
            _context.Conversations
                .Where(c => c.Operator.Identity == query.OperatorIdentity)
                .Select(c => new ConversationResult
                {
                    VisitorInfo = new VisitorInfoResult
                    {
                        Name = c.Visitor.Name,
                        
                    }
                }).ToList();
        }
    }
}