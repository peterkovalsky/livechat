namespace Kookaburra.Domain.Command.Model
{
    public class StartConversationCommand : ICommand
    {
        public string OperatorConnectionId { get; private set; }  

        public string SessionId { get; private set; }

        public string VisitorName { get; private set; }

        public string VisitorEmail { get; set; }

        public string Location { get; set; }

        public string Page { get; set; }

        public StartConversationCommand(string operatorConnectionId, string visitorName, string sessionId)
        {
            OperatorConnectionId = operatorConnectionId;                     
            SessionId = sessionId;
            VisitorName = visitorName;
        }
    }
}