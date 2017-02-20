using System;

namespace Kookaburra.Domain.Query.Result
{
    public class MessageResult
    {
        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime Time { get; set; }

        public string SentBy { get; set; }
    }
}