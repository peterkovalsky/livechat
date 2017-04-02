namespace Kookaburra.Domain.Command.Model
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