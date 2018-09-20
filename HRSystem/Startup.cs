using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HRSystem.Startup))]
namespace HRSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
