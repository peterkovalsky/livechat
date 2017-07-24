using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.TimmedOutConversations
{
    public class TimmedOutConversationsQuery : IQuery<Task<List<string>>>
    {
        public TimmedOutConversationsQuery(int timeoutInMinutes)
        {
            TimeoutInMinutes = timeoutInMinutes;
        }

        public int TimeoutInMinutes { get; }

        public string OperatorIdentity { get; }
    }
}