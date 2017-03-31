namespace Kookaburra.Domain.Command.Model
{
    public class LeaveMessageCommand : ICommand
    {
        public LeaveMessageCommand(string accountKey, string name, string email, string message, string operatorIdentity)
        {
            AccountKey = accountKey;
            Name = name;
            Email = email;
            Message = message;
            OperatorIdentity = operatorIdentity;
        }

        public string AccountKey { get; }

        public string Name { get; }

        public string Email { get; }

        public string Message { get; }

        public string VisitorIP { get; set; }

        public string OperatorIdentity { get; }
    }
}