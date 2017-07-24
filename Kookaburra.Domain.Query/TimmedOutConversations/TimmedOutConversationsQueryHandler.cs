using Kookaburra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.TimmedOutConversations
{
    public class TimmedOutConversationsQueryHandler : IQueryHandler<TimmedOutConversationsQuery, Task<List<string>>>
    {
        private readonly KookaburraContext _context;

        public TimmedOutConversationsQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<List<string>> ExecuteAsync(TimmedOutConversationsQuery query)
        {
            var cutOffTime = DateTime.UtcNow.AddMinutes(-query.TimeoutInMinutes);

            return await _context.Conversations
                .Include(i => i.Visitor)
                .Where(c => c.TimeFinished == null && c.Messages.Any() && c.Messages.OrderByDescending(m => m.DateSent).FirstOrDefault().DateSent < cutOffTime)
                .Select(c => c.Visitor.SessionId)
                .ToListAsync();
        }
    }
}