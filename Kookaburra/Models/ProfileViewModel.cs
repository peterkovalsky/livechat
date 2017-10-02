namespace Kookaburra.Models
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {
            UserDetails = new UserDetailsViewModel();
            ResetPassword = new ResetPasswordViewModel();
        }
        public UserDetailsViewModel UserDetails { get; set; }

        public ResetPasswordViewModel ResetPassword { get; set; } 
    }
}