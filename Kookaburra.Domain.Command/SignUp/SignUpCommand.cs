using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.SignUp
{
    public class SignUpCommand : ICommand
    {
        public string OperatorIdentity { get; set; }

        public string Account { get; set; }
    }
}