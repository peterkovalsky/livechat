using System.Collections.Generic;

namespace Kookaburra.Services.Chats
{
    public class CurrentChatsResponse
    {
        public List<ChatInfoResponse> CurrentChats { get; set; }

        public int UnreadMessages { get; set; }
    }

    public class ChatInfoResponse
    {
        public string VisitorSessionId { get; set; }
    }
}