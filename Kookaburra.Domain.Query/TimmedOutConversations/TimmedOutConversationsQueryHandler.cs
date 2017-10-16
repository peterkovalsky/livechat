using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.TimmedOutConversations
{
    public class TimmedOutConversationsQueryHandler : IQueryHandler<TimmedOutConversationsQuery, Task<TimmedOutConversationsQueryResult>>
    {
        private readonly KookaburraContext _context;

        public TimmedOutConversationsQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<TimmedOutConversationsQueryResult> ExecuteAsync(TimmedOutConversationsQuery query)
        {
            var cutOffTime = DateTime.UtcNow.AddMinutes(-query.TimeoutInMinutes);

            var conversations = await _context.Conversations
                .Include(i => i.Visitor)
                .Where(c => c.TimeFinished == null && c.Messages.Any() && c.Messages.OrderByDescending(m => m.DateSent).FirstOrDefault().DateSent < cutOffTime)
                .Select(c => new ConversationResult
                {
                    VisitorSessionId = c.Visitor.Identifier,
                    ConversationId = c.Id
                })
                .ToListAsync();

            return new TimmedOutConversationsQueryResult
            {
                Conversations = conversations
            };
        }
    }
}