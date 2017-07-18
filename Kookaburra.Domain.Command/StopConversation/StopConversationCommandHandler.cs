using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.StopConversation
{
    public class StopConversationCommandHandler : ICommandHandler<StopConversationCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public StopConversationCommandHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }


        public async Task ExecuteAsync(StopConversationCommand command)
        {
            var visitorSession = _chatSession.GetVisitorByVisitorSessionId(command.VisitorSessionId);
            var conversation = await _context.Conversations.Where(c => c.Id == visitorSession.ConversationId).SingleOrDefaultAsync();

            if (conversation == null)
            {
                throw new ArgumentException("There is no conversation for visitor " + command.VisitorSessionId);
            }

            conversation.TimeFinished = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _chatSession.RemoveVisitor(command.VisitorSessionId);
        }
    }
}