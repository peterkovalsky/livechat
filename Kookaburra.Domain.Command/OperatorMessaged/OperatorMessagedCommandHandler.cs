using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.OperatorMessaged
{
    public class OperatorMessagedCommandHandler : ICommandHandler<OperatorMessagedCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public OperatorMessagedCommandHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }

        public async Task ExecuteAsync(OperatorMessagedCommand command)
        {
            var visitorSession = _chatSession.GetVisitorByVisitorSessionId(command.VisitorSessionId);
            if (visitorSession == null)
            {
                throw new ArgumentException(string.Format("Visitor with session {0} doesn't exist.", command.VisitorSessionId));
            }

            var message = new Message
            {
                ConversationId = visitorSession.ConversationId,
                SentBy = command.SentBy.ToString(),
                Text = command.Message,
                DateSent = command.DateSent
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}