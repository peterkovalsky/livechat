using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.CurrentChats
{
    public class CurrentChatsQuery : IQuery<Task<CurrentChatsQueryResult>>
    {
        public CurrentChatsQuery(string operatorIdentity)
        {
            OperatorIdentity = operatorIdentity;
        }

        public string OperatorIdentity { get; private set; }
    }
}