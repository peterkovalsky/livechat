namespace Kookaburra.Domain.Command.LeaveMessage
{
    public class LeaveMessageCommand : ICommand
    {
        public LeaveMessageCommand(string accountKey, string name, string email, string message)
        {
            AccountKey = accountKey;
            Name = name;
            Email = email;
            Message = message;          
        }

        public string AccountKey { get; }

        public string Name { get; }

        public string Email { get; }

        public string Message { get; }

        public string VisitorIP { get; set; }
    }
}