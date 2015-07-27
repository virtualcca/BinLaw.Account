using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BinLaw.Account.Web.Startup))]
namespace BinLaw.Account.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
