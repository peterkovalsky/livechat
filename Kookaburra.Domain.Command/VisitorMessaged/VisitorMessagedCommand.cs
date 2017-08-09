using System;

namespace Kookaburra.Domain.Command.VisitorMessaged
{
    public class VisitorMessagedCommand : ICommand
    {
        public VisitorMessagedCommand(string visitorConnectionId, string message, DateTime dateSent)
        {
            VisitorConnectionId = visitorConnectionId;            
            Message = message;
            DateSent = dateSent;           
        }

        public string VisitorConnectionId { get; }        

        public string Message { get; }

        public DateTime DateSent { get; }

        public string AccountKey { get; }
    }
}