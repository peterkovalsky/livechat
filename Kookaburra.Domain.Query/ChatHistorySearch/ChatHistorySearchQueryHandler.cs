using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.ChatHistory;
using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.ChatHistorySearch
{
    public class ChatHistorySearchQueryHandler : IQueryHandler<ChatHistorySearchQuery, Task<ChatHistoryQueryResult>>
    {
        private readonly KookaburraContext _context;

        public ChatHistorySearchQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<ChatHistoryQueryResult> ExecuteAsync(ChatHistorySearchQuery query)
        {
            var conversations = _context.Conversations.Where(c =>
                                c.Operator.Account.Identifier == query.AccountKey
                                && c.Messages.Any(m => m.SentBy == UserType.Visitor.ToString())
                                && c.TimeFinished != null);

            conversations = conversations.Where(c =>
                                   c.Messages.Any(m => m.Text.Contains(query.Query))
                                || c.Visitor.Name.Contains(query.Query));

            var total = await conversations.CountAsync();

            if (query.Pagination != null)
            {
                conversations = conversations.OrderByDescending(om => om.TimeStarted).Skip(query.Pagination.Skip).Take(query.Pagination.Size);
            }

            return new ChatHistoryQueryResult
            {
                TotalConversations = total,

                Conversations = await conversations.Select(c => new ConversationItemQueryResult
                {
                    Id = c.Id,
                    VisitorName = c.Visitor.Name,
                    OperatorName = c.Operator.FirstName,
                    Text = c.Messages.FirstOrDefault(m => m.SentBy == UserType.Visitor.ToString()).Text,
                    StartTime = c.TimeStarted,
                    TotalMessages = c.Messages.Count()
                }).ToListAsync()
            };
        }
    }
}