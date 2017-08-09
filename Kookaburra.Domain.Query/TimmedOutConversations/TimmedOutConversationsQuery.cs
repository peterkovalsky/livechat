using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.TimmedOutConversations
{
    public class TimmedOutConversationsQuery : IQuery<Task<TimmedOutConversationsQueryResult>>
    {
        public TimmedOutConversationsQuery(int timeoutInMinutes)
        {
            TimeoutInMinutes = timeoutInMinutes;
        }

        public int TimeoutInMinutes { get; }      

        public string AccountKey { get; }
    }
}