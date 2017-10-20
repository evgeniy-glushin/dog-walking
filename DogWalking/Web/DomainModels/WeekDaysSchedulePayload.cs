using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class WeekDaysSchedulePayload
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime Now { get; set; }
        public List<WorkingDay> WorkingDays { get; set; }
        public List<DogPack> DogPacks { get; set; }
        public TimeSpan WalkDuration { get; set; }
        public List<PriceRate> PriceRates { get; set; }
    }
}
