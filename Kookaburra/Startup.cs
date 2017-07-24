using Hangfire;
using Hangfire.StructureMap;
using Kookaburra.App_Start;
using Kookaburra.DependencyResolution;
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

            var container = IoC.Initialize();
            GlobalConfiguration.Configuration.UseStructureMapActivator(container);

            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            JobsConfig.RegisterJobs();
        }      
    }
}