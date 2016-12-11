using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;

namespace Kookaburra.Domain.Command.Handler
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

        public void Execute(OperatorMessagedCommand command)
        {           
            var message = new Message
            {
                ConversationId = _chatSession.GetConversationId(command.VisitorConnectionId),
                SentBy = UserType.Operator.ToString(),
                Text = command.Message,
                DateSent = command.DateSent
            };

            _context.Messages.Add(message);
            _context.SaveChanges();
        }
    }
}