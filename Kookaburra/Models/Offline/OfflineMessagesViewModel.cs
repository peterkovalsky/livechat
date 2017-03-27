using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Offline
{
    public class OfflineMessagesViewModel
    {
        [JsonProperty("offlineMessages")]
        public List<LeftMessageViewModel> OfflineMessages { get; set; }

        [JsonProperty("totalMessages")]
        public int TotalMessages { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }
}