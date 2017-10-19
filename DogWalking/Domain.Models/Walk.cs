using System;

namespace Domain.Models
{
    public class Walk
    {
        public DateTime StartDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DogPack DogPack { get; set; }
        public decimal Price { get; set; }
    }
}
