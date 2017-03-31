using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
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