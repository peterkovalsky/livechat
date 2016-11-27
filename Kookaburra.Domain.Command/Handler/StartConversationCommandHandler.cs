using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Domain.Command.Handler
{
    public class StartConversationCommandHandler : ICommandHandler<StartConversationCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public StartConversationCommandHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }


        public void Execute(StartConversationCommand command)
        {
            // record new/returning visitor
            var returningVisitor = CheckForVisitor(command.VisitorName, command.VisitorEmail, command.SessionId);
            if (returningVisitor == null)
            {
                returningVisitor = new Visitor
                {
                    Name = command.VisitorName,
                    Email = command.VisitorEmail,
                    Location = command.Location,
                    SessionId = command.SessionId,
                    Page = command.Page,
                    ConversationStarted = DateTime.UtcNow
                };
                _context.Visitors.Add(returningVisitor);
            }

            var conversation = new Conversation
            {
                OperatorId = command.OperatorId,
                Visitor = returningVisitor,
                TimeStarted = DateTime.UtcNow
            };
            _context.Conversations.Add(conversation);

            _context.SaveChanges();

           
            // add visitor to session
            _chatSession.AddVisitor(conversation.Id, command.OperatorConnectionId, command.VisitorConnectionId, returningVisitor.Id, returningVisitor.Name);            
        }

        private Visitor CheckForVisitor(string name, string email, string sessionId)
        {
            var existingVisitor = _context.Visitors
                .Where(v => (v.Name == name && v.Email == email) || (v.SessionId == sessionId))
                .SingleOrDefault();

            return existingVisitor;
        }
    }
}