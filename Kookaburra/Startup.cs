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
            app.MapSignalR();            
        }
    }
}