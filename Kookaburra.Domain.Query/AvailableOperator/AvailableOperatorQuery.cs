using Kookaburra.Domain.Query;
using System.Threading.Tasks;

namespace Kookaburra.Domain.AvailableOperator
{
    public class AvailableOperatorQuery : IQuery<Task<AvailableOperatorQueryResult>>
    {
        public AvailableOperatorQuery(string accountKey)
        {
            AccountKey = accountKey;         
        }

        public string AccountKey { get; }

        public string OperatorIdentity { get; }
    }
}