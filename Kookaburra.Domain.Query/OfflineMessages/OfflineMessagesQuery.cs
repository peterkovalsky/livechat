using Kookaburra.Domain.Common;

namespace Kookaburra.Domain.Query.OfflineMessages
{
    public class OfflineMessagesQuery : IQuery<OfflineMessagesQueryResult>
    {
        public OfflineMessagesQuery(TimeFilterType timeFilter, string operatorIdentity)
        {
            TimeFilter = timeFilter;
            OperatorIdentity = operatorIdentity;
        }

        public TimeFilterType TimeFilter { get; }

        public Pagination Pagination { get; set; }

        public string OperatorIdentity { get; }
    }
}