namespace Kookaburra.Domain.Command.StartVisitorChat
{
    public class StartVisitorChatCommand : ICommand
    {
        public StartVisitorChatCommand(int operatorId, string visitorName, string sessionId, string accountKey)
        {
            OperatorId = operatorId;
            SessionId = sessionId;
            VisitorName = visitorName;
            AccountKey = accountKey;
        }

        public int OperatorId { get; }  

        public string SessionId { get; }

        public string VisitorName { get; }

        public string VisitorEmail { get; set; }

        public string VisitorIP { get; set; }

        public string Page { get; set; }

        public string AccountKey { get; }
    }
}