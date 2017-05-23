using Kookaburra.Email;
using Kookaburra.Email.Public.SignUpWelcome;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Services
{
    public class EmailService
    {
        private readonly KookaburraContext _context;   

        public EmailService(KookaburraContext context)
        {
            _context = context;                
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
        }
    }
}