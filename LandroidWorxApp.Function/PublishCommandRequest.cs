using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static uPLibrary.Networking.M2Mqtt.MqttClient;

namespace LandroidWorxApp.Function
{
    public class PublishCommandRequest
    {
        public string Broker { get; set; }
        public string Uuid { get; set; }
        public string CmdInPath { get; set; }
        public string CmdOutPath { get; set; }
        public string Content { get; set; }
        public string CertWX { get; set; }
    }
}
