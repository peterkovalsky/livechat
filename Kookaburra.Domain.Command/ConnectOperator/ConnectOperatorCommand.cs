namespace Kookaburra.Domain.Command.ConnectOperator
{
    public class ConnectOperatorCommand : ICommand
    {
        public ConnectOperatorCommand(string operatorConnectionId, string operatorIdentity, string accountKey)
        {
            OperatorIdentity = operatorIdentity;
            OperatorConnectionId = operatorConnectionId;
            AccountKey = accountKey;
        }

        public string OperatorIdentity { get; }

        public string OperatorConnectionId { get; }

        public string AccountKey { get; }
    }
}