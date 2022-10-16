using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinalAssignment.Startup))]
namespace FinalAssignment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
