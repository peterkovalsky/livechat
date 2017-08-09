using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.ChatHistory;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.ChatHistorySearch
{
    public class ChatHistorySearchQuery : IQuery<Task<ChatHistoryQueryResult>>
    {
        public ChatHistorySearchQuery(string query, string accountKey)
        {
            Query = query;
            AccountKey = accountKey;
        }

        public string Query { get; }

        public Pagination Pagination { get; set; }

        public string AccountKey { get; }
    }
}