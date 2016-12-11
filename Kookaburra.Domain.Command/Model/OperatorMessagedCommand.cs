using System;

namespace Kookaburra.Domain.Command.Model
{
    public class OperatorMessagedCommand : ICommand
    {
        public OperatorMessagedCommand(string visitorConnectionId, string message, DateTime dateSent)
        {            
            VisitorConnectionId = visitorConnectionId;            
            Message = message;
            DateSent = dateSent;
        }   

        public string VisitorConnectionId { get; private set; }        

        public string Message { get; private set; }

        public DateTime DateSent { get; private set; }
    }
}
