using System;

namespace Kookaburra.Email.Public.OfflineMessage
{
    public class OfflineMessageEmail : IEmailModel
    {
        public string VisitorName { get; set; }

        public DateTime SentDate { get; set; }

        public string Message { get; set; }
    }
}