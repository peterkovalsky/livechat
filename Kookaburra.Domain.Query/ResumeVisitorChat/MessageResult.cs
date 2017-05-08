using System;

namespace Kookaburra.Domain.ResumeVisitorChat
{
    public class MessageResult
    {
        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime SentOn { get; set; }

        public string SentBy { get; set; }
    }
}