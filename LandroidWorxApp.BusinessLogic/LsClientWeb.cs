
using LandroidWorxApp.DataLayer;
using LandroidWorxApp.DataLayer.POCO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace LandroidWorxApp.BusinessLogic
{
    public class LsClientWeb : ILsClientWeb
    {
        private readonly IConfiguration _configuration;
        private readonly RepoManager _repoManager;

        public LsClientWeb(IConfiguration configuration, string connectionStringName)
        {
            // Configuration
            _configuration = configuration;

            // Repository
            _repoManager = new RepoManager(connectionStringName, configuration);
        }

        public LsClientWeb_LoginResponse Login(LsClientWeb_LoginRequest request)
        {
            NameValueCollection nvc = new NameValueCollection
            {
                { "username", request.Username },
                { "password", request.Password },
                { "grant_type", request.GrantType },
                { "client_id", request.ClientId.ToString() },
                { "client_secret", request.ClientSecret },
                { "scope", request.Scope }
            };

            LsClientWeb_LoginResponse response = new LsClientWeb_LoginResponse();

            WebClient client = new WebClient();
            var buf = client.UploadValues(_configuration.GetValue<string>("WorxApi") + "oauth/token", nvc);
            var str = Encoding.UTF8.GetString(buf);
            Debug.WriteLine("Oauth token: {0}", str);
            using (MemoryStream ms = new MemoryStream(buf))
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(LsOAuth));
                LsOAuth lsoa = (LsOAuth)dcjs.ReadObject(ms);

                response.BearerToken = string.Format("{0} {1}", lsoa.Type, lsoa.Token);
            }

            response.BrokerUrl = GetBrokerEndpoint(response.BearerToken);
            response.CertWX = GetMqttCertificate(response.BearerToken);

            // Save Userdata on DB
            _repoManager.GenericOperations.Save(new UserData() { Username = request.Username, X509Certificate2 = Convert.ToBase64String(response.CertWX.Export(X509ContentType.Cert))});

            return response;
        }

        public LsClientWeb_GetProductsResponse GetProducts(LsClientWeb_GetProductsRequest request)
        {
            var client = new WebClient();
            client.Headers["Authorization"] = request.BearerToken;
            var buf = client.DownloadData(_configuration.GetValue<string>("WorxApi") + "product-items");
            var str = Encoding.UTF8.GetString(buf);
            Debug.WriteLine("Product items: {0}", str);

            LsClientWeb_GetProductsResponse response = new LsClientWeb_GetProductsResponse();
            using (MemoryStream ms = new MemoryStream(buf))
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(List<LsProductItem>));
                response.Products = (List<LsProductItem>)dcjs.ReadObject(ms);
            }
            return response;
        }

        public LsClientWeb_GetProductStatusResponse GetProductStatus(LsClientWeb_GetProductStatusRequest request)
        {
            var client = new WebClient();
            client.Headers["Authorization"] = request.BearerToken;
            var buf = client.DownloadData(_configuration.GetValue<string>("WorxApi") + "product-items/" + request.SerianNumber + "/status");
            var str = Encoding.UTF8.GetString(buf);
            Debug.WriteLine("Product Status: {0}", str);

            LsClientWeb_GetProductStatusResponse response = new LsClientWeb_GetProductStatusResponse();
            using (MemoryStream ms = new MemoryStream(buf))
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(LsMqtt));
                LsMqtt jm = (LsMqtt)dcjs.ReadObject(ms);
                response.Status = jm;
            }
            return response;
        }

        public LsClientWeb_PublishCommandResponse PublishCommand (LsClientWeb_PublishCommandRequest request)
        {
            if(request.CertWX == null && !string.IsNullOrEmpty(request.BearerToken))
                request.CertWX = GetMqttCertificate(request.BearerToken);

            MqttClient mqtt = new MqttClient(request.Broker, 8883, true, null, request.CertWX, MqttSslProtocols.TLSv1_2);

            byte code = mqtt.Connect("android-" + request.Uuid);
            Debug.WriteLine(string.Format("Connect '{0} ({1})'", code, mqtt.IsConnected));

            mqtt.Subscribe(new string[] { request.CmdOutPath }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            mqtt.MqttMsgPublished += request.Handler;

            var msgId = mqtt.Publish(request.CmdInPath, Encoding.ASCII.GetBytes(request.Content),  MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            Debug.WriteLine(string.Format("Publish send '{0}'", msgId));

            return new LsClientWeb_PublishCommandResponse() { MessageId = msgId };
        }

        #region PRIVATE METHODS
        private string GetBrokerEndpoint(string token)
        {
            var client = new WebClient();
            client.Headers["Authorization"] = token;
            var buf = client.DownloadData(_configuration.GetValue<string>("WorxApi") + "users/me");
            var str = Encoding.UTF8.GetString(buf);
            Debug.WriteLine("User info: {0}", str);

            string broker = null;
            using (MemoryStream ms = new MemoryStream(buf))
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(LsUserMe));
                LsUserMe ku = (LsUserMe)dcjs.ReadObject(ms);
                broker = ku.Endpoint;
            }
            return broker;
        }

        private X509Certificate2 GetMqttCertificate(string token)
        {
            var client = new WebClient();
            client.Headers["Authorization"] = token;
            var buf = client.DownloadData(_configuration.GetValue<string>("WorxApi") + "users/certificate");
            var str = Encoding.UTF8.GetString(buf);
            Debug.WriteLine("Certificate: {0}", str);

            using (MemoryStream ms = new MemoryStream(buf))
            {
                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(LsCertificate));
                LsCertificate lsc = (LsCertificate)dcjs.ReadObject(ms);
                ms.Close();

                if (!string.IsNullOrEmpty(lsc.Pkcs12))
                {
                    str = lsc.Pkcs12.Replace("\\/", "/");
                    buf = Convert.FromBase64String(str);
                    return new X509Certificate2(buf);
                }
            }

            return null;
        }
        #endregion
    }
}
