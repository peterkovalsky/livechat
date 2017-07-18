namespace Kookaburra.Domain.Command.DeleteMessage
{
    public class DeleteMessageCommand : ICommand
    {
        public DeleteMessageCommand(long messageId, string operatorIdentity)
        {
            MessageId = messageId;
            OperatorIdentity = operatorIdentity;
        }

        public long MessageId { get; }

        public string OperatorIdentity { get; }
    }
}