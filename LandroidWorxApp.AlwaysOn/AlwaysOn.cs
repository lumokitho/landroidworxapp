using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace LandroidWorxApp.AlwaysOn
{
    public static class AlwaysOn
    {
        [FunctionName("AlwaysOn")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            HttpClient pingClient = new HttpClient();
            await pingClient.GetAsync("https://landroidworxapp.azurewebsites.net/");
        }
    }
}
