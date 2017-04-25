using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.ChatHistory;
using Kookaburra.Repository;
using System.Linq;

namespace Kookaburra.Domain.Query.ChatHistorySearch
{
    public class ChatHistorySearchQueryHandler : IQueryHandler<ChatHistorySearchQuery, ChatHistoryQueryResult>
    {
        private readonly KookaburraContext _context;

        public ChatHistorySearchQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public ChatHistoryQueryResult Execute(ChatHistorySearchQuery query)
        {
            var account = _context.Accounts.SingleOrDefault(a => a.Operators.Any(o => o.Identity == query.OperatorIdentity));

            var conversations = _context.Conversations.Where(c =>
                                c.Operator.AccountId == account.Id
                                && c.Messages.Any(m => m.SentBy == UserType.Visitor.ToString())
                                && c.TimeFinished != null);

            conversations = conversations.Where(c =>
                                   c.Messages.Any(m => m.Text.Contains(query.Query))
                                || c.Visitor.Name.Contains(query.Query));

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