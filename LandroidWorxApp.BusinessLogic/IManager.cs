using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LandroidWorxApp.BusinessLogic
{
    public interface IManager
    {
        GetTimePlanningsResponse GetTimePlannings(GetTimePlanningsRequest request);
        SaveTimePlanningsResponse SaveTimePlanningsRequest(SaveTimePlanningsRequest request);
        Task SetTimeCommand(SendTimePlanCommandRequest time);
    }
}
