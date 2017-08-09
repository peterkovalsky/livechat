using Kookaburra.Domain.Common;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.OfflineMessages
{
    public class OfflineMessagesQuery : IQuery<Task<OfflineMessagesQueryResult>>
    {
        public OfflineMessagesQuery(TimeFilterType timeFilter, string accountKey)
        {
            TimeFilter = timeFilter;
            AccountKey = accountKey;
        }

        public TimeFilterType TimeFilter { get; }

        public Pagination Pagination { get; set; }      

        public string AccountKey { get; }
    }
}