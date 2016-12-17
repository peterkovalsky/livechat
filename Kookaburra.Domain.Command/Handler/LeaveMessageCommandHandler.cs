using Kookaburra.Domain.Command.Model;
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

        public LeaveMessageCommandHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }

        public void Execute(LeaveMessageCommand command)
        {
            var account = _context.Accounts.Where(a => a.Identifier == command.AccountKey).SingleOrDefault();

            if (account == null)
            {
                throw new ArgumentException(string.Format("Account {0} doesn't exists", command.AccountKey));
            }

            var offlineMessage = new OfflineMessage
            {
                Message = command.Message,
                DateSent = DateTime.UtcNow,
                IsRead = false,
                AccountId = account.Id,
                Visitor = new Visitor
                {
                    Name = command.Name,
                    Email = command.Email,
                    Location = command.Location
                }
            };

            _context.OfflineMessages.Add(offlineMessage);
            _context.SaveChanges();
        }
    }
}