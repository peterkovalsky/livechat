namespace Kookaburra.Domain.Command.Model
{
    public class ConnectOperatorCommand : ICommand
    {
        public ConnectOperatorCommand(string operatorConnectionId, string operatorIdentity)
        {
            OperatorIdentity = operatorIdentity;
            OperatorConnectionId = operatorConnectionId;
        }

        public string OperatorIdentity { get; private set; }

        public string OperatorConnectionId { get; private set; }
    }
}