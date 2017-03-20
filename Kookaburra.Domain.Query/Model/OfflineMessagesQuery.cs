using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class OfflineMessagesQuery : IQuery<OfflineMessagesQueryResult>
    {
        public OfflineMessagesQuery(TimeFilterType timeFilter)
        {
            TimeFilter = timeFilter;
        }

        public TimeFilterType TimeFilter { get; private set; }

        public Pagination Pagination { get; set; }
    }
}