using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class ChatHistoryQuery : IQuery<ChatHistoryQueryResult>
    {
        public ChatHistoryQuery(TimeFilterType timeFilter, string operatorIdentity)
        {
            TimeFilter = timeFilter;
            OperatorIdentity = operatorIdentity;
        }

        public TimeFilterType TimeFilter { get; }

        public Pagination Pagination { get; set; }

        public string OperatorIdentity { get; }
    }
}