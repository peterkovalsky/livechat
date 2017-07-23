using Hangfire;
using Hangfire.StructureMap;
using Kookaburra.App_Start;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kookaburra.Startup))]
namespace Kookaburra
{
    public partial class Startup
    { 
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            AutoMapperConfig.Initialize();

            app.MapSignalR();

            var container = StructuremapMvc.StructureMapDependencyScope.Container;
            GlobalConfiguration.Configuration.UseStructureMapActivator(container);

            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

           
        }

        private void Jobs()
        {
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Recurring!"), Cron.Minutely());
        }
    }
}