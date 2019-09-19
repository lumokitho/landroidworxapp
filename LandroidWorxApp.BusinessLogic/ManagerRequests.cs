using System;
using System.Collections.Generic;
using System.Text;

namespace LandroidWorxApp.BusinessLogic
{
    public class GetTimePlanningsRequest
    {
        public string Username { get; set; }
    }
    public class GetTimePlanningsResponse
    {
        public List<TimePlanning_BL> Plannings { get; set; }
    }
}
