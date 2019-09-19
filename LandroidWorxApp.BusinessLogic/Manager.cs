using LandroidWorxApp.DataLayer;
using LandroidWorxApp.DataLayer.POCO;
using Mapster;
using Microsoft.Extensions.Configuration;
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
    }
}
