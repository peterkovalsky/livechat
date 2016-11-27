namespace Kookaburra.Domain.Command.Model
{
    public class StartConversationCommand : ICommand
    {
        public int OperatorId { get; private set; }

        public string VisitorConnectionId { get; private set; }

        public string OperatorConnectionId { get; private set; }

        public string SessionId { get; private set; }

        public string VisitorName { get; private set; }

        public string VisitorEmail { get; private set; }

        public string Location { get; set; }

        public string Page { get; set; }

        public StartConversationCommand(int operatorId, string visitorConnectionId, string operatorConnectionId, string sessionId, string visitorName, string visitorEmail)
        {
            OperatorId = operatorId;
            VisitorConnectionId = visitorConnectionId;
            OperatorConnectionId = operatorConnectionId;
            SessionId = sessionId;
            VisitorName = visitorName;
            VisitorEmail = visitorEmail;
        }
    }
}