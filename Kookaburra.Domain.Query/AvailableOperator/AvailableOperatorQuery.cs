using Kookaburra.Domain.Query;

namespace Kookaburra.Domain.AvailableOperator
{
    public class AvailableOperatorQuery : IQuery<AvailableOperatorQueryResult>
    {
        public AvailableOperatorQuery(string accountKey, string operatorIdentity)
        {
            AccountKey = accountKey;
            OperatorIdentity = operatorIdentity;
        }

        public string AccountKey { get; }

        public string OperatorIdentity { get; }
    }
}