using System;

namespace Kookaburra.Services.Chats
{
    public class ChatsPerDayResponse
    {
        public DateTime Day { get; set; }

        public int TotalChats { get; set; }
    }
}