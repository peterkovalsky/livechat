using Kookaburra.Domain.Common;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.ChatHistory
{
    public class ChatHistoryQuery : IQuery<Task<ChatHistoryQueryResult>>
    {
        public ChatHistoryQuery(TimeFilterType timeFilter, string accountKey)
        {
            TimeFilter = timeFilter;
            AccountKey = accountKey;
        }

        public TimeFilterType TimeFilter { get; }

        public Pagination Pagination { get; set; }    

        public string AccountKey { get; }
    }
}