using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FIT5132_MyModelFirst.Startup))]
namespace FIT5132_MyModelFirst
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
