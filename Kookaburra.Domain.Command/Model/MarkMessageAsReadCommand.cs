namespace Kookaburra.Domain.Command.Model
{
    public class MarkMessageAsReadCommand : ICommand
    {
        public MarkMessageAsReadCommand(int messageId, string operatorIdentity)
        {
            MessageId = messageId;
            OperatorIdentity = operatorIdentity;
        }

        public int MessageId { get; }

        public string OperatorIdentity { get; }
    }
}