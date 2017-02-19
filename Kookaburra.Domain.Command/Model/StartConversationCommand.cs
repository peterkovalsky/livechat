namespace Kookaburra.Domain.Command.Model
{
    public class StartConversationCommand : ICommand
    {
        public int OperatorId { get; private set; }  

        public string SessionId { get; private set; }

        public string VisitorName { get; private set; }

        public string VisitorEmail { get; set; }

        public string VisitorIP { get; set; }

        public string Page { get; set; }

        public StartConversationCommand(int operatorId, string visitorName, string sessionId)
        {
            OperatorId = operatorId;                     
            SessionId = sessionId;
            VisitorName = visitorName;
        }
    }
}