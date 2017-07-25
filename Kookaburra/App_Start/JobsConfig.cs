using Hangfire;

namespace Kookaburra.App_Start
{
    public class JobsConfig
    {
        public static void RegisterJobs()
        {
            //RecurringJob.AddOrUpdate<BackgroundJobs>(backgroundJobs => backgroundJobs.TimeoutInactiveConversations(), Cron.MinuteInterval(5));
            RecurringJob.AddOrUpdate<BackgroundJobs>(backgroundJobs => backgroundJobs.TimeoutInactiveConversations(), Cron.Minutely());
        }
    }
}