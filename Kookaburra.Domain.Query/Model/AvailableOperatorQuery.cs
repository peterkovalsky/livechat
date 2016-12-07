using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class AvailableOperatorQuery : IQuery<AvailableOperatorQueryResult>
    {
        public AvailableOperatorQuery(string accountKey)
        {
            AccountKey = accountKey;
        }

        public string AccountKey { get; private set; }
    }
}