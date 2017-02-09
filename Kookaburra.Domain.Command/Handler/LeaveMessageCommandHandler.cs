using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Integration;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Domain.Command.Handler
{
    public class LeaveMessageCommandHandler : ICommandHandler<LeaveMessageCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;
        private readonly IGeoLocator _geoLocator;

        public LeaveMessageCommandHandler(KookaburraContext context, ChatSession chatSession, IGeoLocator geoLocator)
        {
            _context = context;
            _chatSession = chatSession;
            _geoLocator = geoLocator;
        }

        public void Execute(LeaveMessageCommand command)
        {
            var account = _context.Accounts.Where(a => a.Identifier == command.AccountKey).SingleOrDefault();

            if (account == null)
            {
                throw new ArgumentException(string.Format("Account {0} doesn't exists", command.AccountKey));
            }

            var location = _geoLocator.GetLocation(command.VisitorIP);        

            var offlineMessage = new OfflineMessage
            {
                Message = command.Message,
                DateSent = DateTime.UtcNow,
                IsRead = false,
                AccountId = account.Id,
                Visitor = new Visitor
                {
                    Name = command.Name,
                    Email = command.Email            
                }
            };

            if (location != null)
            {
                offlineMessage.Visitor.Country = location.Country;
                offlineMessage.Visitor.Region = location.Region;
                offlineMessage.Visitor.City = location.City;
                offlineMessage.Visitor.Latitude = location.Latitude;
                offlineMessage.Visitor.Longitude = location.Longitude;
            }

            _context.OfflineMessages.Add(offlineMessage);
            _context.SaveChanges();
        }
    }
}