namespace Kookaburra.Domain.Command.StopConversation
{
    public class StopConversationCommand : ICommand
    {
        public StopConversationCommand(string visitorSessionId)
        {
            VisitorSessionId = visitorSessionId;           
        }

        public string VisitorSessionId { get; }

        public long ConversationId { get; set; }     

        public string AccountKey { get; }
    }
}