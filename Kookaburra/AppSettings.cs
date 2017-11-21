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

        public static string UrlPortal
        {
            get
            {
                return ConfigurationManager.AppSettings["Url.Portal"];
            }
        }

        public static string UrlWebsite
        {
            get
            {
                return ConfigurationManager.AppSettings["Url.Website"];
            }
        }

        public static int TrialPeriodDays
        {
            get
            {
                int trialPeriod = 0;
                var trialPeiodSetting = ConfigurationManager.AppSettings["Kookaburra.TrialPeriodDays"];
                int.TryParse(trialPeiodSetting, out trialPeriod);

                return trialPeriod;
            }
        }
    }
}