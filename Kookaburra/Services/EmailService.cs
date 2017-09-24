using Kookaburra.Email;
using Kookaburra.Email.Public.SignUpWelcome;
using Kookaburra.Repository;
using System;
using System.Linq;
using System.Data.Entity;
using Kookaburra.Email.Public.OfflineMessage;

namespace Kookaburra.Services
{
    public class EmailService
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

        public void SendSignUpWelcomeEmail(string operatorIdentity)
        {
            var owner = _context.Operators.Where(o => o.Identity == operatorIdentity).SingleOrDefault();
            if (owner == null)
            {
                throw new ArgumentException($"Operator with id {operatorIdentity} doesn't exists");
            }

          
            var to = new AddressInfo(owner.Email);

            var model = new SignUpWelcomeEmail
            {
                FirstName = owner.FirstName
            };

            _mailer.SendEmail(_from, to, model);
        }

        public void SendOfflineMessageEmail(long id)
        {
            var offlineMessage = _context.OfflineMessages.Include(i => i.Visitor).Where(om => om.Id == id).SingleOrDefault();
            if (offlineMessage == null)
            {
                throw new ArgumentException($"Offline message with ID {id} doesn't exist");
            }

            var operators1 = _context.OfflineMessages.Include(i => i.Visitor).Where(om => om.Id == id).Select(om => om.Visitor.Account.Operators).ToList();

            var operators = _context.Operators.Where(o => o.Account.Visitors.Any(v => v.OfflineMessages.Any(om => om.Id == id))).ToList();

            foreach (var oper in operators)
            {
                var to = new AddressInfo(oper.Email);

                var model = new OfflineMessageEmail
                {
                    VisitorName = offlineMessage.Visitor.Name,
                    Message = offlineMessage.Message,
                    SentDate = offlineMessage.DateSent
                };

                _mailer.SendEmail(_from, to, model);
            }           
        }
    }
}