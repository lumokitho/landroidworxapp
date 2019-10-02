using Hangfire;
using LandroidWorxApp.DataLayer;
using LandroidWorxApp.DataLayer.POCO;
using Mapster;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace LandroidWorxApp.BusinessLogic
{
    public class Manager : IManager
    {
        private readonly IConfiguration _configuration;
        private readonly RepoManager _repoManager;

        public Manager(IConfiguration configuration, string connectionStringName)
        {
            _configuration = configuration;
            _repoManager = new RepoManager(connectionStringName, configuration);
        }


        public GetTimePlanningsResponse GetTimePlannings(GetTimePlanningsRequest request)
        {
            var plannings = _repoManager.GenericOperations.GetByExpression<TimePlanning>(t => t.Username == request.Username);
            return new GetTimePlanningsResponse() { Plannings = plannings.ConvertAll(c => c.Adapt<TimePlanning_BL>()) };
        }

        public SaveTimePlanningsResponse SaveTimePlanningsRequest(SaveTimePlanningsRequest request)
        {
            // Get old time plannings of user and remove all with recurring jobs 
            var oldPlannings = _repoManager.GenericOperations.GetByExpression<TimePlanning>(p => p.Username == request.Username);
            oldPlannings.ForEach(p => { RecurringJob.RemoveIfExists(p.Id.ToString()); });
            _repoManager.GenericOperations.DeleteAll(oldPlannings);
            
            var newPlannings = _repoManager.GenericOperations.SaveAll(request.Plannings.ConvertAll(c => c.Adapt<TimePlanning>()));

            newPlannings.ForEach(p => { p.TimeStart = p.TimeStart.Subtract(TimeSpan.FromMinutes(5)); RecurringJob.AddOrUpdate<IManager>(p.Id.ToString(), (m) => m.Test(), Cron.Weekly(p.DayOfWeek, p.TimeStart.Hours, p.TimeStart.Minutes ), TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time")); });
            return new SaveTimePlanningsResponse()
            {
                PlanningsUpdated = newPlannings.ConvertAll(c => c.Adapt<TimePlanning_BL>())
            };
        }

        public void Test()
        {

        }
    }
}
