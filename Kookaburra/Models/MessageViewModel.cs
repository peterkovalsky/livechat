using Newtonsoft.Json;
using System;

namespace Kookaburra.Models
{
    public class MessageViewModel
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("sentBy")]
        public string SentBy { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }
    }
}