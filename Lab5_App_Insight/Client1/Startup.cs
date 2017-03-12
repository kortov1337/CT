using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Client1.Startup))]
namespace Client1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
