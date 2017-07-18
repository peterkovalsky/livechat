using Kookaburra.Domain.Common;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.ChatHistory
{
    public class ChatHistoryQuery : IQuery<Task<ChatHistoryQueryResult>>
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