using Kookaburra.Domain.Common;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Domain.Query.ChatHistory
{
    public class ChatHistoryQueryHandler : IQueryHandler<ChatHistoryQuery, ChatHistoryQueryResult>
    {
        private readonly KookaburraContext _context;

        public ChatHistoryQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public ChatHistoryQueryResult Execute(ChatHistoryQuery query)
        {
            var account = _context.Accounts.SingleOrDefault(a => a.Operators.Any(o => o.Identity == query.OperatorIdentity));

            var conversations = _context.Conversations.Where(c => 
                                c.Operator.AccountId == account.Id
                                && c.Messages.Any(m => m.SentBy == UserType.Visitor.ToString())
                                && c.TimeFinished != null);

            if (query.TimeFilter == TimeFilterType.Today)
            {
                var aDayAgo = DateTime.UtcNow.AddDays(-1);

                conversations = conversations.Where(c => aDayAgo <= c.TimeStarted);
            }
            else if (query.TimeFilter == TimeFilterType.Week)
            {
                var aWeekAgo = DateTime.UtcNow.AddDays(-7);

                conversations = conversations.Where(c => aWeekAgo <= c.TimeStarted);
            }
            else if (query.TimeFilter == TimeFilterType.Fortnight)
            {
                var aFortnightAgo = DateTime.UtcNow.AddDays(-14);

                conversations = conversations.Where(c => aFortnightAgo <= c.TimeStarted);
            }
            else if (query.TimeFilter == TimeFilterType.Month)
            {
                var aMonthAgo = DateTime.UtcNow.AddMonths(-1);

                conversations = conversations.Where(c => aMonthAgo <= c.TimeStarted);
            }
            else if (query.TimeFilter == TimeFilterType.Year)
            {
                var aYearAgo = DateTime.UtcNow.AddYears(-1);

                conversations = conversations.Where(c => aYearAgo <= c.TimeStarted);
            }
            else if (query.TimeFilter == TimeFilterType.All)
            {
            }

            var total = conversations.Count();

            if (query.Pagination != null)
            {
                conversations = conversations.OrderByDescending(om => om.TimeStarted).Skip(query.Pagination.Skip).Take(query.Pagination.Size);
            }

            return new ChatHistoryQueryResult
            {
                TotalConversations = total,

                Conversations = conversations.Select(c => new ConversationItemQueryResult
                {
                    Id = c.Id,
                    VisitorName = c.Visitor.Name,
                    OperatorName = c.Operator.FirstName,
                    Text = c.Messages.FirstOrDefault(m => m.SentBy == UserType.Visitor.ToString()).Text,
                    StartTime = c.TimeStarted,
                    TotalMessages = c.Messages.Count()
                }).ToList()
            };
        }
    }
}