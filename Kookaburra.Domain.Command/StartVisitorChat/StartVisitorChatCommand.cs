namespace Kookaburra.Domain.Command.StartVisitorChat
{
    public class StartVisitorChatCommand : ICommand
    {
        public StartVisitorChatCommand(int operatorId, string visitorName, string sessionId, string operatorIdentity)
        {
            OperatorId = operatorId;
            SessionId = sessionId;
            VisitorName = visitorName;
            OperatorIdentity = operatorIdentity;
        }

        public int OperatorId { get; }  

        public string SessionId { get; }

        public string VisitorName { get; }

        public string VisitorEmail { get; set; }

        public string VisitorIP { get; set; }

        public string Page { get; set; }

        public string OperatorIdentity { get; }     
    }
}