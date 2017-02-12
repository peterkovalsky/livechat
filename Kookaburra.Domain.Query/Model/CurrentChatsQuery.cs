using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class CurrentChatsQuery : IQuery<CurrentChatsQueryResult>
    {
        public CurrentChatsQuery(string operatorIdentity)
        {
            OperatorIdentity = operatorIdentity;
        }

        public string OperatorIdentity { get; private set; }
    }
}