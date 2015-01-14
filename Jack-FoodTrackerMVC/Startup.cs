using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Jack_FoodTrackerMVC.Startup))]
namespace Jack_FoodTrackerMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
