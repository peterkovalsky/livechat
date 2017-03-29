using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class SearchOfflineMessagesQuery : IQuery<OfflineMessagesQueryResult>
    {
        public SearchOfflineMessagesQuery(string query)
        {
            Query = query;
        }

        public string Query { get; private set; }

        public Pagination Pagination { get; set; }
    }
}