using System.Collections.Generic;

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

        public AlertViewModel Alert { get; set; }
    }
}