using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;
using System.Linq;

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
            var conversation = _context.Conversations
                                        .Where(c => c.Operator.Identity == query.OperatorIdentity)
                                        .Select(c => new ConversationResult
                                        {
                                            VisitorInfo = new VisitorInfoResult
                                            {
                                                Name = c.Visitor.Name,
                                                CurrentUrl = c.Page,
                                                TimeStarted = c.TimeStarted,
                                                Location = @"{c.Visitor.Country}, {c.Visitor.City}",
                                                Latitude = c.Visitor.Latitude,
                                                Longitude = c.Visitor.Longitude
                                            },
                                            Messages = c.Messages.Select(m => new MessageResult
                                            {
                                                Author = m.SentBy == UserType.Visitor.ToString() ? c.Visitor.Name : c.Operator.FirstName,
                                                Text = m.Text,
                                                Time = m.DateSent,
                                                SentBy = m.SentBy.ToLower()
                                            }).ToList()
                                        }).ToList();

            return new ResumeOperatorQueryResult { Conversations = conversation };
        }
    }
}