using Kookaburra.Email;
using Kookaburra.Email.Public.OfflineMessage;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Services
{
    public class EmailService : IEmailService
    {
        private readonly KookaburraContext _context;
        private readonly IMailer _mailer;
        private readonly AddressInfo _from;
        private readonly string _bcc;        
            
        public EmailService(KookaburraContext context, IMailer mailer)
        {
            _context = context;
            _mailer = mailer;

            _from = new AddressInfo("Kookaburra Chat", "info@kookaburra.chat");
            _bcc = "it@kookaburra.chat";            
        }

        public void SendOfflineNotificationEmail(long messageId)
        {
            var messageEmailModel = _context.OfflineMessages
                .Where(o => o.Id == messageId)
                .Select(o => new OfflineMessageEmail
                {
                    AccountId = o.Visitor.AccountId,
                    Website = o.Visitor.Account.Name,
                    VisitorName = o.Visitor.Name,
                    VisitorEmail = o.Visitor.Email,
                    Page = o.Page,                    
                    Message = o.Message,
                    DateSent = o.DateSent
                })
                .SingleOrDefault();

            if (messageEmailModel == null)
            {
                throw new ArgumentException($"Offline message with id {messageId} doesn't exist");
            }          

            var operators = _context.Operators.Where(o => o.Account.Id == messageEmailModel.AccountId).ToList();

            // send to every operator attached to an account
            foreach (var op in operators)
            {
                var to = new AddressInfo(op.Email);               

                _mailer.SendEmail(_from, to, messageEmailModel.VisitorEmail, messageEmailModel, _bcc);
            }
        }
    }
}