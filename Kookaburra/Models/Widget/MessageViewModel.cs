using Newtonsoft.Json;
using System;

namespace Kookaburra.Models.Widget
{
    public class MessageViewModel
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }
}