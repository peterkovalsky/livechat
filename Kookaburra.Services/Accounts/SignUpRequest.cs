namespace Kookaburra.Services.Accounts
{
    public class SignUpRequest
    {
        public string ClientName { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string OperatorIdentity { get; set; }

        public int TrialPeriodDays { get; set; }
    }
}