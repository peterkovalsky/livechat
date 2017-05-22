using System.Configuration;

namespace Kookaburra
{
    public class AppSettings
    {
        public static string EmailHost
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.Host"];
            }
        }

        public static string EmailUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.Username"];
            }
        }

        public static string EmailPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.Password"];
            }
        }
    }
}