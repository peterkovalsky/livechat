using Kookaburra.Repository;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.Profile
{
    public class ProfileQueryHandler : IQueryHandler<ProfileQuery, Task<ProfileQueryResult>>
    {
        private readonly KookaburraContext _context;

        public ProfileQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<ProfileQueryResult> ExecuteAsync(ProfileQuery query)
        {
            var op = await _context.Operators.SingleOrDefaultAsync(o => o.Identifier == query.OperatorKey);

            return new ProfileQueryResult
            {
                FirstName = op.FirstName,
                LastName = op.LastName,
                Email = op.Email
            };
        }
    }
}