namespace Kookaburra.Domain.Command.Model
{
    public class StopConversationCommand : ICommand
    {
        public StopConversationCommand(string visitorSessionId)
        {
            VisitorSessionId = visitorSessionId;
        }

        public string VisitorSessionId { get; private set; }
    }
}