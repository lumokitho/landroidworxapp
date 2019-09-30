using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Hangfire;
using System.Configuration;

[assembly: OwinStartup(typeof(Dex.Console.HangfireDasboard.Startup))]
namespace Dex.Console.HangfireDasboard
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireDb", new Hangfire.SqlServer.SqlServerStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(Int32.Parse(ConfigurationManager.AppSettings["HangfireQueuePollIntervalSeconds"]))
            });
            app.UseHangfireDashboard();
        }
    }
}
