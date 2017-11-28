using Newtonsoft.Json;

namespace Kookaburra.Models.Home
{
    public class ChatsPerDayViewModel
    {
        [JsonProperty("day")]
        public double Day { get; set; }

        [JsonProperty("chats")]
        public int TotalChats { get; set; }
    }
}