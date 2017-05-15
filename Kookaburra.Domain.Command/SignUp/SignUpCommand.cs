namespace Kookaburra.Domain.Command.SignUp
{
    public class SignUpCommand : ICommand
    {        
        public string ClientName { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public string OperatorIdentity { get; set; }        
    }
}