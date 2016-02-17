using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EPMSAppDemo.Startup))]
namespace EPMSAppDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
