using Kookaburra.Domain.Integration;
using Kookaburra.Domain.Model;
using Kookaburra.Email;
using Kookaburra.Email.Public.OfflineMessage;
using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.LeaveMessage
{
    public class LeaveMessageCommandHandler : ICommandHandler<LeaveMessageCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;
        private readonly IGeoLocator _geoLocator;
        private readonly IMailer _mailer;

        public LeaveMessageCommandHandler(KookaburraContext context, ChatSession chatSession, IGeoLocator geoLocator, IMailer mailer)
        {
            _context = context;
            _chatSession = chatSession;
            _geoLocator = geoLocator;
            _mailer = mailer;
        }

        public async Task ExecuteAsync(LeaveMessageCommand command)
        {
            var account = await _context.Accounts.Where(a => a.Identifier == command.AccountKey).SingleOrDefaultAsync();

            if (account == null)
            {
                throw new ArgumentException(string.Format("Account {0} doesn't exists", command.AccountKey));
            }

            var offlineMessage = new OfflineMessage
            {
                Message = command.Message,
                DateSent = DateTime.UtcNow,
                IsRead = false,
                Visitor = new Visitor
                {
                    Name = command.Name,
                    Email = command.Email,
                    IpAddress = command.VisitorIP,
                    AccountId = account.Id
                }
            };

            try
            {
                var location = await _geoLocator.GetLocationAsync(command.VisitorIP);

                if (location != null)
                {
                    offlineMessage.Visitor.Country = location.Country;
                    offlineMessage.Visitor.CountryCode = location.CountryCode;
                    offlineMessage.Visitor.Region = location.Region;
                    offlineMessage.Visitor.City = location.City;
                    offlineMessage.Visitor.Latitude = location.Latitude;
                    offlineMessage.Visitor.Longitude = location.Longitude;
                }
            }
            catch (Exception ex)
            {
                // log geo location exception   
            }

            _context.OfflineMessages.Add(offlineMessage);
            await _context.SaveChangesAsync();

            SendEmailNotification(offlineMessage, command.AccountKey);
        }

        /// <summary>
        /// Send email notification to all operators about new offline message
        /// </summary>
        private void SendEmailNotification(OfflineMessage offlineMessage, string accountKey)
        {
            try
            {
                var operators = _context.Operators.Where(o => o.Account.Identifier == accountKey).ToList();
                var from = new AddressInfo("Kookaburra Chat", "info@kookaburra.chat");

                foreach (var op in operators)
                {
                    var to = new AddressInfo(op.Email);

                    var model = new OfflineMessageEmail
                    {
                        VisitorName = offlineMessage.Visitor.Name,
                        VisitorEmail = offlineMessage.Visitor.Email,
                        Page = offlineMessage.Page,
                        QuestionLink = $"htt",
                        Question = offlineMessage.Message,
                        SentDate = offlineMessage.DateSent
                    };

                    try
                    {
                        _mailer.SendEmail(from, to, model);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}