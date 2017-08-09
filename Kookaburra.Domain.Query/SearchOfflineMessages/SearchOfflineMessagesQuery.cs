using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.OfflineMessages;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.SearchOfflineMessages
{
    public class SearchOfflineMessagesQuery : IQuery<Task<OfflineMessagesQueryResult>>
    {
        public SearchOfflineMessagesQuery(string query, string accountKey)
        {
            Query = query;
            AccountKey = accountKey;
        }

        public string Query { get; }

        public Pagination Pagination { get; set; }     

        public string AccountKey { get; }
    }
}