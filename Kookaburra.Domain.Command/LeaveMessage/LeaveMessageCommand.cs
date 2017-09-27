namespace Kookaburra.Domain.Command.LeaveMessage
{
    public class LeaveMessageCommand : ICommand
    {
        public LeaveMessageCommand(string accountKey, string name, string email, string message, string portalHost)
        {
            AccountKey = accountKey;
            Name = name;
            Email = email;
            Message = message;
            PortalHost = portalHost;
        }

        public long Id { get; set; }

        public string AccountKey { get; }

        public string Name { get; }

        public string Email { get; }

        public string Page { get; set; }

        public string Message { get; }

        public string VisitorIP { get; set; }

        public string PortalHost { get; }

        public string VisitorId { get; set; }
    }
}