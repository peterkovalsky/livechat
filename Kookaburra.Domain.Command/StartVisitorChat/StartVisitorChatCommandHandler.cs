using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Integration;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Domain.Command.StartVisitorChat
{
    public class StartVisitorChatCommandHandler : ICommandHandler<StartVisitorChatCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;
        private readonly IGeoLocator _geoLocator;

        public StartVisitorChatCommandHandler(KookaburraContext context, ChatSession chatSession, IGeoLocator geoLocator)
        {
            _context = context;
            _chatSession = chatSession;
            _geoLocator = geoLocator;
        }


        public void Execute(StartVisitorChatCommand command)
        {
            // record new/returning visitor
            var returningVisitor = CheckForVisitor(command.VisitorName, command.VisitorEmail, command.SessionId);

            // new visitor
            if (returningVisitor == null)
            {                         
                returningVisitor = new Visitor
                {
                    Name = command.VisitorName,
                    Email = command.VisitorEmail,             
                    SessionId = command.SessionId,
                    IpAddress = command.VisitorIP,
                };

                try
                {
                    var location = _geoLocator.GetLocation(command.VisitorIP);

                    if (location != null)
                    {
                        returningVisitor.Country = location.Country;
                        returningVisitor.CountryCode = location.CountryCode;
                        returningVisitor.Region = location.Region;
                        returningVisitor.City = location.City;
                        returningVisitor.Latitude = location.Latitude;
                        returningVisitor.Longitude = location.Longitude;
                    }
                }
                catch (Exception ex)
                {
                    // log geo location exception
                }            

                _context.Visitors.Add(returningVisitor);
            }

            var operatorSession = _chatSession.GetOperatorById(command.OperatorId);

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
            _chatSession.AddVisitor(conversation.Id, command.OperatorId, null, returningVisitor.Id, returningVisitor.Name, returningVisitor.SessionId);            
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