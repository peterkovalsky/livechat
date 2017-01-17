using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;

namespace Kookaburra.Domain.Command.Handler
{
    public class VisitorMessagedCommandHandler : ICommandHandler<VisitorMessagedCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public VisitorMessagedCommandHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }

        public void Execute(VisitorMessagedCommand command)
        {
            var visitorSession = _chatSession.GetVisitorByVisitorConnId(command.VisitorConnectionId);
            var message = new Message
            {
                ConversationId = visitorSession.ConversationId,
                SentBy = UserType.Visitor.ToString(),
                Text = command.Message,
                DateSent = command.DateSent
            };

            _context.Messages.Add(message);
            _context.SaveChanges();
        }
    }
}