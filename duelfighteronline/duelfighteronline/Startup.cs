using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(duelfighteronline.Startup))]
namespace duelfighteronline
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
