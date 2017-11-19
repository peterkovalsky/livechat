using System;

namespace Kookaburra.Services.Chats
{
    public class MessageResponse
    {
        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime SentOn { get; set; }

        public string SentBy { get; set; }
    }
}