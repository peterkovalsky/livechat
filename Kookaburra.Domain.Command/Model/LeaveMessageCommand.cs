namespace Kookaburra.Domain.Command.Model
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

        public string AccountKey { get; private set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public string Message { get; private set; }

        public string Location { get; set; }
    }
}