namespace Kookaburra.Domain.Command.DeleteMessage
{
    public class DeleteMessageCommand : ICommand
    {
        public DeleteMessageCommand(long messageId, string accountKey)
        {
            MessageId = messageId;
            AccountKey = accountKey;
        }

        public long MessageId { get; }   

        public string AccountKey { get; }
    }
}