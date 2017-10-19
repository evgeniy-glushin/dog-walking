using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class WorkingDay
    {
        public DayOfWeek DayOfWeek { get; set; }
        public List<TimeSpan> WorkingHours { get; set; }
    }
}
