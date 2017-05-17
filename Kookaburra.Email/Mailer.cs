using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kookaburra.Email
{
    public class Mailer : IMailer
    {
        public void SendEmail<T>(T model) where T : IEmailModel
        {

        }

        public string GetTemplate<T>(T model) where T : IEmailModel
        {
            var assembly = Assembly.GetExecutingAssembly();
            Type myType = model.GetType();
            var n = myType.Namespace;

            var resourceName = $"{assembly.FullName}.{myType.Namespace}.html";

            string template = string.Empty;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                template = reader.ReadToEnd();
            }

            return template;
        }
    }
}