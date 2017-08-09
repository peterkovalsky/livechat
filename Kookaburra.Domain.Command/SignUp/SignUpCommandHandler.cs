using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.SignUp
{
    public class SignUpCommandHandler : ICommandHandler<SignUpCommand>
    {
        private readonly KookaburraContext _context;

        public SignUpCommandHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(SignUpCommand command)
        {
            var account = new Account
            {
                Identifier = command.AccountKey,
                Name = command.Company,
                Operators = new List<Operator>
                {
                    new Operator
                    {
                        FirstName = command.ClientName,
                        Email = command.Email,
                        Identity = command.OperatorIdentity,
                        Type = OperatorType.OWNER.ToString()
                    }
                }
            };            

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }
    }
}