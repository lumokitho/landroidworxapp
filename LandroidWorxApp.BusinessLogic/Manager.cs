using Hangfire;
using LandroidWorxApp.DataLayer;
using LandroidWorxApp.DataLayer.POCO;
using Mapster;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace LandroidWorxApp.BusinessLogic
{
    public class Manager : IManager
    {
        private readonly IConfiguration _configuration;
        private readonly RepoManager _repoManager;
        private readonly ILsClientWeb _lsClientWeb;

        public Manager(IConfiguration configuration, string connectionStringName)
        {
            _configuration = configuration;
            _repoManager = new RepoManager(connectionStringName, configuration);
            _lsClientWeb = new LsClientWeb(configuration, connectionStringName);
        }


        public GetTimePlanningsResponse GetTimePlannings(GetTimePlanningsRequest request)
        {
            var plannings = _repoManager.GenericOperations.GetByExpression<TimePlanning>(t => t.Username == request.Username);
            return new GetTimePlanningsResponse() { Plannings = plannings.ConvertAll(c => c.Adapt<TimePlanning_BL>()) };
        }

        public SaveTimePlanningsResponse SaveTimePlanningsRequest(SaveTimePlanningsRequest request)
        {
            // Get old time plannings of user and remove all with recurring jobs 
            var oldPlannings = _repoManager.GenericOperations.GetByExpression<TimePlanning>(p => p.Username == request.Plannings.First().Username);
            oldPlannings.ForEach(p => { RecurringJob.RemoveIfExists(p.Id.ToString()); });
            _repoManager.GenericOperations.DeleteAll(oldPlannings);
            
            var newPlannings = _repoManager.GenericOperations.SaveAll(request.Plannings.ConvertAll(c => c.Adapt<TimePlanning>()));

            //newPlannings.ForEach(p => { p.TimeStart = p.TimeStart.Subtract(TimeSpan.FromMinutes(5)); RecurringJob.AddOrUpdate<IManager>(p.Id.ToString(), (m) => m.Test(new SendTimePlanCommandRequest() { SerialNumber = request.SerialNumber, Planning = p.Adapt<TimePlanning_BL>() }), Cron.Weekly(p.DayOfWeek, p.TimeStart.Hours, p.TimeStart.Minutes ), TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time")); });
            return new SaveTimePlanningsResponse()
            {
                PlanningsUpdated = newPlannings.ConvertAll(c => c.Adapt<TimePlanning_BL>())
            };
        }

        public void Test(SendTimePlanCommandRequest command)
        {
            UserData user = _repoManager.GenericOperations.GetSingleByExpression<UserData>(u => u.Username == command.Planning.Username);
            UserProduct product = _repoManager.GenericOperations.GetSingleByExpression<UserProduct>(p => p.SerialNumber == command.SerialNumber);

            X509Certificate2 certificate = new X509Certificate2(Convert.FromBase64String(user.X509Certificate2));
            var zoneCommand = "{\"mzv\":[{0},{0},{0},{0},{0},{0},{0},{0},{0},{0}]}".Replace("{0}", command.Planning.Zone.ToString());
            var planCommand = string.Format("{{\"sc\":{{\"d\":[[{0}],[{1}],[{2}],[{3}],[{4}],[{5}],[{6}]],\"m\":1,\"p\":-100}}}}", 
                command.Planning.DayOfWeek == DayOfWeek.Sunday ? string.Format("\"{0}:{1}\",{2},{3}", command.Planning.TimeStart.Hours, command.Planning.TimeStart.Minutes, command.Planning.Duration, command.Planning.CutEdge ? 1 : 0) : "\"00:00\",0,0",
                command.Planning.DayOfWeek == DayOfWeek.Monday ? string.Format("\"{0}:{1}\",{2},{3}", command.Planning.TimeStart.Hours, command.Planning.TimeStart.Minutes, command.Planning.Duration, command.Planning.CutEdge ? 1 : 0) : "\"00:00\",0,0",
                command.Planning.DayOfWeek == DayOfWeek.Tuesday ? string.Format("\"{0}:{1}\",{2},{3}", command.Planning.TimeStart.Hours, command.Planning.TimeStart.Minutes, command.Planning.Duration, command.Planning.CutEdge ? 1 : 0) : "\"00:00\",0,0",
                command.Planning.DayOfWeek == DayOfWeek.Wednesday ? string.Format("\"{0}:{1}\",{2},{3}", command.Planning.TimeStart.Hours, command.Planning.TimeStart.Minutes, command.Planning.Duration, command.Planning.CutEdge ? 1 : 0) : "\"00:00\",0,0",
                command.Planning.DayOfWeek == DayOfWeek.Thursday ? string.Format("\"{0}:{1}\",{2},{3}", command.Planning.TimeStart.Hours, command.Planning.TimeStart.Minutes, command.Planning.Duration, command.Planning.CutEdge ? 1 : 0) : "\"00:00\",0,0",
                command.Planning.DayOfWeek == DayOfWeek.Friday ? string.Format("\"{0}:{1}\",{2},{3}", command.Planning.TimeStart.Hours, command.Planning.TimeStart.Minutes, command.Planning.Duration, command.Planning.CutEdge ? 1 : 0) : "\"00:00\",0,0",
                command.Planning.DayOfWeek == DayOfWeek.Saturday ? string.Format("\"{0}:{1}\",{2},{3}", command.Planning.TimeStart.Hours, command.Planning.TimeStart.Minutes, command.Planning.Duration, command.Planning.CutEdge ? 1 : 0) : "\"00:00\",0,0"
                );
            _lsClientWeb.PublishCommand(new LsClientWeb_PublishCommandRequest()
            {
                CertWX = certificate,
                Content = planCommand,
                CmdInPath = product.CmdInPath,
                CmdOutPath = product.CmdOutPath,
                Broker = user.Broker,
                Uuid = Guid.NewGuid().ToString(),
                Handler = (object sender, MqttMsgPublishedEventArgs e) =>
                {
                    ((MqttClient)sender).Disconnect();
                }
            });
            _lsClientWeb.PublishCommand(new LsClientWeb_PublishCommandRequest()
            {
                CertWX = certificate,
                Content = zoneCommand,
                CmdInPath = product.CmdInPath,
                CmdOutPath = product.CmdOutPath,
                Broker = user.Broker,
                Uuid = Guid.NewGuid().ToString(),
                Handler = (object sender, MqttMsgPublishedEventArgs e) =>
                {
                    ((MqttClient)sender).Disconnect();
                }
            });
        }
    }
}
