using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
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

        public async Task<Account> GetAccountAsync(string operatorIdentity)
        {
            return await _context.Accounts.Where(a => a.Operators.Any(o => o.Identity == operatorIdentity)).SingleOrDefaultAsync();
        }

        public async Task<Operator> GetOperatorAsync(string operatorIdentity)
        {
            return await _context.Operators.Include(i => i.Account).SingleOrDefaultAsync(o => o.Identity == operatorIdentity);
        }

        public async Task UpdateProfileAsync(string operatorIdentity, string firstName, string lastName, string email)
        {
            var operatr = await GetOperatorAsync(operatorIdentity);
            if (operatr == null)
            {
                throw new ArgumentException($"Operator with id {operatorIdentity} doesn't exist");
            }

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