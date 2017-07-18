using Kookaburra.Domain.Common;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.OfflineMessages
{
    public class OfflineMessagesQuery : IQuery<Task<OfflineMessagesQueryResult>>
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