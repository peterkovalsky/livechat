using System.Collections.Generic;

namespace Kookaburra.Models.History
{
    public class ChatHistoryViewModel
    {
        public string VisitorName { get; set; }

        public List<MessageViewModel> Conversation { get; set; }
    }
}