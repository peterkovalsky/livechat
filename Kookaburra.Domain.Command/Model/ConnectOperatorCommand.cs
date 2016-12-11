namespace Kookaburra.Domain.Command.Model
{
    public class ConnectOperatorCommand : ICommand
    {
        public ConnectOperatorCommand(string operatorConnectionId, string operatorKey)
        {
            OperatorKey = operatorKey;
            OperatorConnectionId = operatorConnectionId;
        }

        public string OperatorKey { get; private set; }

        public string OperatorConnectionId { get; private set; }
    }
}