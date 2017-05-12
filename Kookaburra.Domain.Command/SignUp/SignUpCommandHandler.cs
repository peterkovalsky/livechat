using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void Execute(SignUpCommand command)
        {
            var account = new Account
            {
                Identifier = Guid.NewGuid().ToString(),
                Name = command.Company,
                Operators = new List<Operator> {
                    new Operator{
                        FirstName = 
                    }
                }
            };
            account.

            _context.Accounts.Add(account);
        }
    }
}