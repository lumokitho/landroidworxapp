﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace LandroidWorxApp.DataLayer.POCO
{
    [SugarTable("TimePlannings")]
    public class TimePlanning
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan TimeStart { get; set; }
        public int Duration { get; set; }
        public int? Zone { get; set; }
        public bool CutEdge { get; set; }
    }
}
