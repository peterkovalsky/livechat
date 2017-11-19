using System.Collections.Generic;

namespace Kookaburra.Services.Chats
{
    public class ResumeVisitorChatResponse
    {
        public OperatorInfoResponse OperatorInfo { get; set; }

        public VisitorInfoResponse VisitorInfo { get; set; }

        public List<MessageResponse> Conversation { get; set; }
    }
}