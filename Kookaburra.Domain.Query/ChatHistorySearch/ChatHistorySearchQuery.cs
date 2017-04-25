using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.ChatHistory;

namespace Kookaburra.Domain.Query.ChatHistorySearch
{
    public class ChatHistorySearchQuery : IQuery<ChatHistoryQueryResult>
    {
        public ChatHistorySearchQuery(string query, string operatorIdentity)
        {
            Query = query;
            OperatorIdentity = operatorIdentity;
        }

        public string Query { get; }

        public Pagination Pagination { get; set; }

        public string OperatorIdentity { get; }
    }
}