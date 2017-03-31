using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
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