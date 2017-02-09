using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Integration;
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
        private readonly IGeoLocator _geoLocator;

        public StartConversationCommandHandler(KookaburraContext context, ChatSession chatSession, IGeoLocator geoLocator)
        {
            _context = context;
            _chatSession = chatSession;
            _geoLocator = geoLocator;
        }


        public void Execute(StartConversationCommand command)
        {
            // record new/returning visitor
            var returningVisitor = CheckForVisitor(command.VisitorName, command.VisitorEmail, command.SessionId);
            if (returningVisitor == null)
            {
                var location = _geoLocator.GetLocation(command.VisitorIP);         

                returningVisitor = new Visitor
                {
                    Name = command.VisitorName,
                    Email = command.VisitorEmail,             
                    SessionId = command.SessionId                                     
                };

                if (location != null)
                {
                    returningVisitor.Country = location.Country;
                    returningVisitor.Region = location.Region;
                    returningVisitor.City = location.City;
                    returningVisitor.Latitude = location.Latitude;
                    returningVisitor.Longitude = location.Longitude;
                }

                _context.Visitors.Add(returningVisitor);
            }

            var operatorSession = _chatSession.GetOperatorByOperatorConnId(command.OperatorConnectionId);

            var conversation = new Conversation
            {
                OperatorId = operatorSession.Id,
                Visitor = returningVisitor,
                TimeStarted = DateTime.UtcNow,
                Page = command.Page
            };
            _context.Conversations.Add(conversation);

            _context.SaveChanges();

           
            // add visitor to session
            _chatSession.AddVisitor(conversation.Id, command.OperatorConnectionId, null, returningVisitor.Id, returningVisitor.Name, returningVisitor.SessionId);            
        }

        private Visitor CheckForVisitor(string name, string email, string sessionId)
        {
            var existingVisitor = _context.Visitors
                .Where(v => v.SessionId == sessionId)
                .SingleOrDefault();

            return existingVisitor;
        }
    }
}