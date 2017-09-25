using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Email;
using Kookaburra.Email.Public.SignUpWelcome;
using Kookaburra.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.SignUp
{
    public class SignUpCommandHandler : ICommandHandler<SignUpCommand>
    {
        private readonly KookaburraContext _context;
        private readonly IMailer _mailer;

        public SignUpCommandHandler(KookaburraContext context, IMailer mailer)
        {
            _context = context;
            _mailer = mailer;
        }

        public async Task ExecuteAsync(SignUpCommand command)
        {
            var accountKey = Guid.NewGuid().ToString();

            var operatr = new Operator
            {
                FirstName = command.ClientName,
                Email = command.Email,
                Identity = command.OperatorIdentity,
                Type = OperatorType.OWNER.ToString()
            };

            var account = new Account
            {
                Identifier = accountKey,
                Name = command.Company,
                Operators = new List<Operator> { operatr }
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            SendEmailNotification(operatr);
        }

        private void SendEmailNotification(Operator operatr)
        {
            try
            {
                var from = new AddressInfo("Kookaburra Chat", "info@kookaburra.chat");
                var to = new AddressInfo(operatr.Email);

                var model = new SignUpWelcomeEmail
                {
                    FirstName = operatr.FirstName
                };

                _mailer.SendEmail(from, to, model);
            }
            catch { }
        }
    }
}