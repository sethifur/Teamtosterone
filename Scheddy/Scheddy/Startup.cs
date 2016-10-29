using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Scheddy.Startup))]
namespace Scheddy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
