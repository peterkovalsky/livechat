namespace Kookaburra.Domain.Command.StopConversation
{
    public class StopConversationCommand : ICommand
    {
        public StopConversationCommand(string visitorSessionId, string operatorIdentity)
        {
            VisitorSessionId = visitorSessionId;
            OperatorIdentity = operatorIdentity;
        }

        public string VisitorSessionId { get; }

        public string OperatorIdentity { get; }
    }
}