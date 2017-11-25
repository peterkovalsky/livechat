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
                    Key = Guid.NewGuid().ToString(),
                    Website = request.Website,
                    SignUpDate = DateTime.UtcNow,
                    IsTrial = true,
                    TrialPeriodDays = request.TrialPeriodDays,
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

        public async Task<AccountStatusType> CheckAccountAsync(string operatorIdentity)
        {
            var account = await GetAccountForOperatorAsync(operatorIdentity);
            var trialExpiryDate = account.SignUpDate.AddDays(account.TrialPeriodDays);

            // trial
            if (account.IsTrial)
            {
                if (trialExpiryDate >= DateTime.UtcNow)
                {
                    return AccountStatusType.Trial;
                }
                else
                {
                    return AccountStatusType.TrialExpired;
                }
            }
            else // paid
            {
                return AccountStatusType.Paid;
            }          
        }

        public async Task<Account> GetAccountAsync(string accountKey)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Key == accountKey);
            if (account == null)
            {
                throw new ArgumentException(string.Format("Account '{0}' doesn't exists", accountKey));
            }

            return account;
        }

        public async Task<Account> GetAccountForOperatorAsync(string operatorIdentity)
        {
            return await _context.Accounts.Where(a => a.Operators.Any(o => o.Identifier == operatorIdentity)).SingleOrDefaultAsync();
        }

        public async Task<Operator> GetOperatorAsync(string operatorIdentity)
        {
            var operatr = await _context.Operators.Include(i => i.Account).SingleOrDefaultAsync(o => o.Identifier == operatorIdentity);
            if (operatr == null)
            {
                throw new ArgumentException($"Operator '{operatorIdentity}' doesn't exist");
            }

            return operatr;
        }

        public async Task UpdateProfileAsync(string operatorIdentity, string firstName, string lastName, string email)
        {
            var operatr = await GetOperatorAsync(operatorIdentity);

            operatr.FirstName = firstName;
            operatr.LastName = lastName;
            operatr.Email = email;

            await _context.SaveChangesAsync();
        }

        public async Task RecordOperatorActivityAsync(string operatorIdentity)
        {
            var operatr = await GetOperatorAsync(operatorIdentity);
            if (operatr == null)
            {
                throw new ArgumentException($"Operator with id {operatorIdentity} doesn't exist");
            }

            operatr.LastActivity = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ResetOperatorActivityAsync(string operatorIdentity)
        {
            var operatr = await GetOperatorAsync(operatorIdentity);
            if (operatr == null)
            {
                throw new ArgumentException($"Operator with id {operatorIdentity} doesn't exist");
            }

            operatr.LastActivity = null;

            await _context.SaveChangesAsync();
        }
    }
}