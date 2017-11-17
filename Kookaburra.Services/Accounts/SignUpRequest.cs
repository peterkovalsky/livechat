namespace Kookaburra.Services.Accounts
{
    public class SignUpRequest
    {
        public string ClientName { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public string OperatorIdentity { get; set; }      
    }
}