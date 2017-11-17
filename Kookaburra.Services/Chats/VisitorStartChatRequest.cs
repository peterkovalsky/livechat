namespace Kookaburra.Services.Chats
{
    public class VisitorStartChatRequest
    {
        public int OperatorId { get; set; }

        public string VisitorKey { get; set; }

        public string VisitorName { get; set; }

        public string VisitorEmail { get; set; }

        public string VisitorIP { get; set; }

        public string Page { get; set; }

        public string AccountKey { get; set; } 
    }
}