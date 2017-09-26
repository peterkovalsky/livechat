using Kookaburra.Email;
using Kookaburra.Email.Public.OfflineMessage;
using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;

namespace Kookaburra.Services
{
    public class EmailService : IEmailService
    {
        private readonly KookaburraContext _context;
        private readonly IMailer _mailer;
        private readonly AddressInfo _from;

        public EmailService(KookaburraContext context, IMailer mailer)
        {
            _context = context;
            _mailer = mailer;

            _from = new AddressInfo("Kookaburra Chat", "info@kookaburra.chat");
        }

        public void SendOfflineNotificationEmail(long offlineMessageId)
        {
            var offlineMessage = _context.OfflineMessages.Include(i => i.Visitor).SingleOrDefault(om => om.Id == offlineMessageId);
            if (offlineMessage == null)
            {
                throw new ArgumentException($"Offline message with id {offlineMessageId} doesn't exist");
            }

            var operators = _context.Operators.Where(o => o.Account.Id == offlineMessage.Visitor.AccountId).ToList();

            foreach (var op in operators)
            {
                var to = new AddressInfo(op.Email);

                var model = new OfflineMessageEmail
                {
                    VisitorName = offlineMessage.Visitor.Name,
                    VisitorEmail = offlineMessage.Visitor.Email,
                    Page = offlineMessage.Page,
                    Question = offlineMessage.Message,
                    SentDate = offlineMessage.DateSent
                };


                _mailer.SendEmail(_from, to, model);
            }
        }
    }
}