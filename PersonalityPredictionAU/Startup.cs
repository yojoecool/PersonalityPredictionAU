using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PersonalityPredictionAU.Startup))]
namespace PersonalityPredictionAU
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
