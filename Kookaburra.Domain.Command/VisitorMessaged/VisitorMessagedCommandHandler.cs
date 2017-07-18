using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.VisitorMessaged
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

        public async Task ExecuteAsync(VisitorMessagedCommand command)
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
            await _context.SaveChangesAsync();
        }
    }
}