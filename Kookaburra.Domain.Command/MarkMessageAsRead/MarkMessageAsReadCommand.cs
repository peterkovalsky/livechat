namespace Kookaburra.Domain.Command.MarkMessageAsRead
{
    public class MarkMessageAsReadCommand : ICommand
    {
        public MarkMessageAsReadCommand(int messageId, string accountKey)
        {
            MessageId = messageId;
            AccountKey = accountKey;
        }

        public int MessageId { get; }  

        public string AccountKey { get; }
    }
}