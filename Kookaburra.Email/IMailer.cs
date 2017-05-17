using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kookaburra.Email
{
    public interface IMailer
    {
        void SendEmail<T>(T model) where T : IEmailModel;


    }
}
