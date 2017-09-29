using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Data.Entity;
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

        public async Task<Operator> GetProfileAsync(string operatorKey)
        {
            return await _context.Operators.SingleOrDefaultAsync(o => o.Identity == operatorKey);
        }

        public async Task UpdateProfileAsync(string operatorKey, string firstName, string lastName, string email)
        {
            var op = await GetProfileAsync(operatorKey);
            if (op == null)
            {
                throw new ArgumentException($"Operator with id {operatorKey} doesn't exist");
            }

            op.FirstName = firstName;
            op.LastName = lastName;
            op.Email = email;

            await _context.SaveChangesAsync();
        }
    }
}