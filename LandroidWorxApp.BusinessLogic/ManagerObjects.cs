using System;
using System.Collections.Generic;
using System.Text;

namespace LandroidWorxApp.BusinessLogic
{
    public class TimePlanning_BL
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan TimeStart { get; set; }
        public string TimeStartString { get; set; }
        public int Duration { get; set; }
        public int Zone { get; set; }
    }
}
