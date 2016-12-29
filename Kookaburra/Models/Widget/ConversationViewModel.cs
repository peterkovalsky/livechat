using System;
using System.Collections.Generic;

namespace Kookaburra.Models.Widget
{
    public class ConversationViewModel
    {
        public List<ConversationItemViewModel> Conversation { get; set; }
    }

    public class ConversationItemViewModel
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public DateTime TimeSent { get; set; }
    }
}