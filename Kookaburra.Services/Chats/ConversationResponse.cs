using System.Collections.Generic;

namespace Kookaburra.Services.Chats
{
    public class ConversationResponse
    {
        public VisitorInfoResponse VisitorInfo { get; set; }

        public List<MessageResponse> Messages { get; set; }
    }
}