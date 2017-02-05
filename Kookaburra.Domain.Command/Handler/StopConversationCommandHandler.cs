using Kookaburra.Domain.Command.Model;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Domain.Command.Handler
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


        public void Execute(StopConversationCommand command)
        {
            var visitorSession = _chatSession.GetVisitorByVisitorSessionId(command.VisitorSessionId);
            var conversation = _context.Conversations.Where(c => c.Id == visitorSession.ConversationId).SingleOrDefault();

            if (conversation == null)
            {
                throw new ArgumentException("There is no conversation for visitor " + command.VisitorSessionId);
            }

            conversation.TimeFinished = DateTime.UtcNow;

            _context.SaveChanges();

            _chatSession.RemoveVisitor(command.VisitorSessionId);
        }
    }
}