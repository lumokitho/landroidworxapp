using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace LandroidWorxApp.Function
{
    public static class PublishCommand
    {
        [FunctionName("PublishCommand")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                PublishCommandRequest request = JsonConvert.DeserializeObject<PublishCommandRequest>(requestBody);

                var cert = new X509Certificate2(Convert.FromBase64String(request.CertWX), (string)null, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

                MqttClient mqtt = new MqttClient(request.Broker, 8883, true, null, cert, MqttSslProtocols.TLSv1_2);

                byte code = mqtt.Connect("android-" + request.Uuid);
                log.LogInformation(string.Format("Connect '{0} ({1})'", code, mqtt.IsConnected));

                mqtt.Subscribe(new string[] { request.CmdOutPath }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                mqtt.MqttMsgPublished += (object sender, MqttMsgPublishedEventArgs e) =>
                {
                    ((MqttClient)sender).Disconnect();
                };

                var msgId = mqtt.Publish(request.CmdInPath, Encoding.ASCII.GetBytes(request.Content), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                log.LogInformation(string.Format("Publish send '{0}'", msgId));

                return new OkObjectResult(msgId);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

        }
    }
}
