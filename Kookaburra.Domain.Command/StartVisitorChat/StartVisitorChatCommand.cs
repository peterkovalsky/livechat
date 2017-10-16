namespace Kookaburra.Domain.Command.StartVisitorChat
{
    public class StartVisitorChatCommand : ICommand
    {
        public StartVisitorChatCommand(int operatorId, string visitorName, string visitorKey, string accountKey)
        {
            OperatorId = operatorId;
            VisitorKey = visitorKey;
            VisitorName = visitorName;
            AccountKey = accountKey;
        }

        public int OperatorId { get; }  

        public string VisitorKey { get; }

        public string VisitorName { get; }

        public string VisitorEmail { get; set; }

        public string VisitorIP { get; set; }

        public string Page { get; set; }

        public string AccountKey { get; }

        public long VisitorId { get; set; }
    }
}