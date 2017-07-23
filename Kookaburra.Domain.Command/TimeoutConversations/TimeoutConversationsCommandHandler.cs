using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.TimeoutConversations
{
    public class TimeoutConversationsCommandHandler : ICommandHandler<TimeoutConversationsCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public TimeoutConversationsCommandHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }

        public async Task ExecuteAsync(TimeoutConversationsCommand command)
        {
            var cutOffTime = DateTime.UtcNow.AddMinutes(-command.TimeoutInMinutes);

            var timmedOutConversations = await _context.Conversations
                .Include(i => i.Visitor)
                .Where(c => c.TimeFinished == null && c.Messages.Any() && c.Messages.OrderByDescending(m => m.DateSent).FirstOrDefault().DateSent < cutOffTime)
                .ToListAsync();

            foreach (var conversation in timmedOutConversations)
            {
                conversation.TimeFinished = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _chatSession.RemoveVisitor(conversation.Visitor.SessionId);
            }
        }
    }
}