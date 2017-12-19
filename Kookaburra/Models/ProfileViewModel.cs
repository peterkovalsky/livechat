namespace Kookaburra.Models
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {
            UserDetails = new UserDetailsViewModel();
            ResetPassword = new ResetUserPasswordViewModel();
        }
        public UserDetailsViewModel UserDetails { get; set; }

        public ResetUserPasswordViewModel ResetPassword { get; set; }

        public AlertViewModel Alert { get; set; }
    }
}