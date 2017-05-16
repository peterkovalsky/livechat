using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Collections.Generic;

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
            _context.SaveChanges();
        }
    }
}