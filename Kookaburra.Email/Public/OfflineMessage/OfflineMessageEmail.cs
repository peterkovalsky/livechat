using System;

namespace Kookaburra.Email.Public.OfflineMessage
{
    public class OfflineMessageEmail : IEmailModel
    {
        public int AccountId { get; set; }

        public string Website { get; set; }

        public string VisitorName { get; set; }

        public string VisitorEmail { get; set; } 

        public string Page { get; set; }

        public DateTime DateSent { get; set; }

        public string Message { get; set; }
    }
}