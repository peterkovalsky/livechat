using Kookaburra.Common;
using Kookaburra.Domain.Common;
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
        public string SentByFormatted
        {
            get
            {
                return SentBy.ToString().ToLower();
            }
        }

        [JsonProperty("time")]
        public double Time
        {
            get
            {
                return SentOn.JsDateTime();
            }
        }

        public UserType SentBy { get; set; }

        public DateTime SentOn { get; set; }
    }
}