namespace Kookaburra.Email
{
    public class EmailMessage
    {
        public string DisplayName { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; }
    }
}