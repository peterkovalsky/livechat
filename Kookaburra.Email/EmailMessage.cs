using System.Collections.Generic;

namespace Kookaburra.Email
{
    public class EmailMessage
    {
        public AddressInfo From { get; set; }

        public AddressInfo To { get; set; }

        public string Bcc { get; set; }

        public string ReplyTo { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; }

        public List<Attachment> Attachments { get; set; }
    }

    public class AddressInfo
    {
        public AddressInfo(string email)
        {
            Email = email;
        }

        public AddressInfo(string displayName, string email)
        {
            DisplayName = displayName;
            Email = email;
        }

        public string DisplayName { get; set; }

        public string Email { get; set; }
    }

    public class Attachment
    {
        public byte[] File { get; set; }

        public string Filename { get; set; }

        public string MIME { get; set; }
    }
}