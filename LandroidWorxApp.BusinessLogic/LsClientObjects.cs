using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LandroidWorxApp.BusinessLogic
{
    #region Structs
    /*
    User auth {"id":000,"name":"...","email":"...","created_at":"...","updated_at":"...","city":null,"address":null,"zipcode":null,
                "country_id":276,"phone":null,"birth_date":null,"sex":null,"newsletter_subscription":null,"user_type":"customer",
                "api_token":"...","token_expiration":"...", "mqtt_client_id":"android-..."}
    */
    [DataContract]
    public struct WorxUser
    {
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "email")]
        public string Email;
        [DataMember(Name = "api_token")]
        public string ApiToken;
        [DataMember(Name = "mqtt_client_id")]
        public string MqttClientId;
        [DataMember(Name = "mqtt_endpoint")]
        public string MqttEndpoint;
    }
    [DataContract]
    public struct LsOAuth
    {
        [DataMember(Name = "access_token")]
        public string Token;
        [DataMember(Name = "expires_in")]
        public int Expires;
        [DataMember(Name = "token_type")]
        public string Type;
    }
    [DataContract]
    public struct LsUserMe
    {
        [DataMember(Name = "mqtt_endpoint")] public string Endpoint;
    }
    [DataContract]
    public struct LsCertificate
    {
        [DataMember(Name = "pkcs12")]
        public string Pkcs12;
    }
    [DataContract]
    public struct LsMqttTopic
    {
        [DataMember(Name = "command_in")]
        public string CmdIn;
        [DataMember(Name = "command_out")]
        public string CmdOut;
    }
    [DataContract]
    public struct LsProductItem
    {
        [DataMember(Name = "serial_number")]
        public string SerialNo;
        [DataMember(Name = "mac_address")]
        public string MacAdr;
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "firmware_auto_upgrade")]
        public bool AutoUpgd;
        [DataMember(Name = "mqtt_topics")]
        public LsMqttTopic Topic;
    }
    [DataContract]
    public struct LsMqtt
    {
        [DataMember(Name = "cfg")]
        public Config Cfg;
        [DataMember(Name = "dat")]
        public Data Dat;
    }

    [DataContract]
    public struct LsJson
    {
        [DataMember(Name = "email")] public string Email;
        [DataMember(Name = "pass")] public string Password;
        [DataMember(Name = "uuid")] public string Uuid;
        [DataMember(Name = "name")] public string Name;
        [DataMember(Name = "broker")] public string Broker;
        [DataMember(Name = "mac")] public string MacAdr;
        [DataMember(Name = "board")] public string Board;
        [DataMember(Name = "blade")] public int Blade;
        [DataMember(Name = "top")] public bool Top;
        [DataMember(Name = "x")] public int X;
        [DataMember(Name = "y")] public int Y;
        [DataMember(Name = "w")] public int W;
        [DataMember(Name = "h")] public int H;
        [DataMember(Name = "plugins")] public List<string> Plugins;

        public bool Equals(LsJson lsj)
        {
            bool b;

            b = Email == lsj.Email && Password == lsj.Password && Uuid == lsj.Uuid && Name == lsj.Name && Broker == lsj.Broker && MacAdr == lsj.MacAdr;
            b = b && Top == lsj.Top && X == lsj.X && Y == lsj.Y && W == lsj.W && H == lsj.H && Blade == lsj.Blade;
            if (b && Plugins != null && lsj.Plugins != null)
            {
                b = Plugins.Count == lsj.Plugins.Count;
                for (int i = 0; b && i < Plugins.Count; i++) b = b && Plugins[i] == lsj.Plugins[i];
            }
            return b;
        }
    }

    [DataContract]
    public struct LsEstimatedTime
    {
        [DataMember(Name = "beg")] public float Beg;
        [DataMember(Name = "end")] public float End;
        [DataMember(Name = "vpm")] public float VoltPerMin;
    }

    [DataContract]
    public struct LsEstimatedTimes
    {
        [DataMember(Name = "home_0")] public LsEstimatedTime HomeOff;
        [DataMember(Name = "home_1")] public LsEstimatedTime HomeOn;
        [DataMember(Name = "mowing")] public LsEstimatedTime Mowing;
    }
    #endregion
}
