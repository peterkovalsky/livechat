using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.OfflineMessages;

namespace Kookaburra.Domain.Query.SearchOfflineMessages
{
    public class SearchOfflineMessagesQuery : IQuery<OfflineMessagesQueryResult>
    {
        public SearchOfflineMessagesQuery(string query, string operatorIdentity)
        {
            Query = query;
            OperatorIdentity = operatorIdentity;
        }

        public string Query { get; }

        public Pagination Pagination { get; set; }

        public string OperatorIdentity { get; }
    }
}