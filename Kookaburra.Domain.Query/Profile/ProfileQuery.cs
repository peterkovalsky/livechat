using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.Profile
{
    public class ProfileQuery : IQuery<Task<ProfileQueryResult>>
    {
        public string OperatorKey { get; set; }

        public string AccountKey { get; }
    }
}