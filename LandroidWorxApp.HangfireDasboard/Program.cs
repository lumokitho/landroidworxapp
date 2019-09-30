using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dex.Console.HangfireDasboard
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:50000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();

                var response = client.GetAsync(baseAddress + "api/values").Result;

                System.Console.WriteLine(baseAddress + "/hangfire");
                //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                System.Console.ReadLine();
            }
        }
    }
}
