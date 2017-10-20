using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class DogWalker
    {
        public int Id { get; set; }
        public List<Customer> Customers { get; set; }
        public List<PriceRate> PriceRates { get; set; }
        public List<WorkingDay> WorkingDays { get; set; }
        public List<Walk> BookedWalks { get; set; }
        public List<DogPack> DogPacks { get; set; }
        public TimeSpan DefaultWalkDuration { get; set; }
    }
}
