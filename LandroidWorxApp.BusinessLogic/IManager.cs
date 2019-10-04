using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LandroidWorxApp.BusinessLogic
{
    public interface IManager
    {
        GetTimePlanningsResponse GetTimePlannings(GetTimePlanningsRequest request);
        SaveTimePlanningsResponse SaveTimePlanningsRequest(SaveTimePlanningsRequest request);
        void SetTimeCommand(SendTimePlanCommandRequest time);
    }
}
