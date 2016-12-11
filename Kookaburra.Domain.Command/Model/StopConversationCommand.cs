namespace Kookaburra.Domain.Command.Model
{
    public class StopConversationCommand : ICommand
    {
        public StopConversationCommand(string visitorConnectionId)
        {
            VisitorConnectionId = visitorConnectionId;
        }

        public string VisitorConnectionId { get; private set; }
    }
}