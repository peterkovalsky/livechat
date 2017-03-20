using System.Collections.Generic;

namespace Kookaburra.Models.Offline
{
    public class OfflineMessagesViewModel
    {
        public List<LeftMessageViewModel> OfflineMessages { get; set; }

        public int TotalMessages { get; set; }
    }
}