using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly KookaburraContext _context;

        public AccountService(KookaburraContext context)
        {
            _context = context;
        }


        public async Task SignUpAsync(SignUpRequest request)
        {
            _context.Accounts.Add(new Account
                {
                    Identifier = Guid.NewGuid().ToString(),
                    Name = request.Company,
                    Operators = new List<Operator> { new Operator
                    {
                        FirstName = request.ClientName,
                        Email = request.Email,
                        Identifier = request.OperatorIdentity,
                        Type = OperatorType.OWNER.ToString()
                    }
                }
            });

            await _context.SaveChangesAsync();
        }

        public async Task<Account> GetAccountAsync(string accountKey)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Identifier == accountKey);
            if (account == null)
            {
                throw new ArgumentException(string.Format("Account '{0}' doesn't exists", accountKey));
            }

            return account;
        }

        public async Task<Account> GetAccountForOperatorAsync(string operatorKey)
        {
            return await _context.Accounts.Where(a => a.Operators.Any(o => o.Identifier == operatorKey)).SingleOrDefaultAsync();
        }

        public async Task<Operator> GetOperatorAsync(string operatorKey)
        {
            var operatr = await _context.Operators.Include(i => i.Account).SingleOrDefaultAsync(o => o.Identifier == operatorKey);
            if (operatr == null)
            {
                throw new ArgumentException($"Operator '{operatorKey}' doesn't exist");
            }

            return operatr;
        }

        public async Task UpdateProfileAsync(string operatorKey, string firstName, string lastName, string email)
        {
            var operatr = await GetOperatorAsync(operatorKey);

            operatr.FirstName = firstName;
            operatr.LastName = lastName;
            operatr.Email = email;

            await _context.SaveChangesAsync();
        }

        public async Task RecordOperatorActivityAsync(string operatorKey)
        {
            var operatr = await GetOperatorAsync(operatorKey);
            if (operatr == null)
            {
                throw new ArgumentException($"Operator with id {operatorKey} doesn't exist");
            }

            operatr.LastActivity = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ResetOperatorActivityAsync(string operatorKey)
        {
            var operatr = await GetOperatorAsync(operatorKey);
            if (operatr == null)
            {
                throw new ArgumentException($"Operator with id {operatorKey} doesn't exist");
            }

            operatr.LastActivity = null;

            await _context.SaveChangesAsync();
        }
    }
}