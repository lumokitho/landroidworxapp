﻿using System;
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

    public class SaveTimePlanningsRequest
    {
        public string SerialNumber { get; set; }
        public List<TimePlanning_BL> Plannings { get; set; }
        public int WorkPercentage { get; set; }
    }
    public class SaveTimePlanningsResponse
    {
        public List<TimePlanning_BL> PlanningsUpdated { get; set; }
    }

    public class SendTimePlanCommandRequest 
    {
        public TimePlanning_BL Planning { get; set; }
        public string SerialNumber { get; set; }
        public int WorkPercentage { get; set; }
    }
}
