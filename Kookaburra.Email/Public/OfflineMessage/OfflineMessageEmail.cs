using System;

namespace Kookaburra.Email.Public.OfflineMessage
{
    public class OfflineMessageEmail : IEmailModel
    {
        public string Website { get; set; }

        public string VisitorName { get; set; }

        public string VisitorEmail { get; set; }

        public string QuestionLink { get; set; }

        public string Page { get; set; }

        public DateTime SentDate { get; set; }

        public string Question { get; set; }
    }
}