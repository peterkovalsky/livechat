using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.CurrentChats
{
    public class CurrentChatsQuery : IQuery<Task<CurrentChatsQueryResult>>
    {
        public CurrentChatsQuery(string operatorIdentity, string accountKey)
        {
            OperatorIdentity = operatorIdentity;
            AccountKey = accountKey;
        }

        public string OperatorIdentity { get; }

        public string AccountKey { get; }
    }
}