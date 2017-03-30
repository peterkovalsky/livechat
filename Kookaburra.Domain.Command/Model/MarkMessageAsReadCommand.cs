namespace Kookaburra.Domain.Command.Model
{
    public class MarkMessageAsReadCommand : ICommand
    {
        public MarkMessageAsReadCommand(int messageId)
        {
            MessageId = messageId;
        }

        public int MessageId { get; private set; }
    }
}