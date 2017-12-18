using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kookaburra.Email.Public.ForgotPassword
{
    public class ForgotPassword : IEmailModel
    {
        public string Email { get; set; }

        public string ResetLink { get; set; }
    }
}