using System.Collections.Generic;

namespace Kookaburra.Domain.Query.OfflineMessages
{
    public class OfflineMessagesQueryResult
    {
        public List<OfflineMessageResult> OfflineMessages { get; set; }

        public int TotalMessages { get; set; }
    }
}