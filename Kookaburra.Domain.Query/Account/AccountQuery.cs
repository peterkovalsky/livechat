using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.Account
{
    public class AccountQuery : IQuery<Task<AccountQueryResult>>
    {
        public AccountQuery(string operatorIdentity)
        {
            OperatorIdentity = operatorIdentity;
        }

        public string OperatorIdentity { get; }

        public string AccountKey { get; }
    }
}