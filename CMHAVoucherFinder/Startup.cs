using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMHAVoucherFinder.Startup))]
namespace CMHAVoucherFinder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
