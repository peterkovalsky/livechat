using System.Collections.Generic;

namespace Kookaburra.ViewModels
{
    public class ConversationViewModel
    {
        public List<MessageViewModel> Messages { get; set; }
    }

    public class MessageViewModel
    {
        public string User { get; set; }

        public string Message { get; set; }
    }
}