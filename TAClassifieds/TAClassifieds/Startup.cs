using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TAClassifieds.Startup))]
namespace TAClassifieds
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
