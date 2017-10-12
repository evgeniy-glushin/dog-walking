using System;
using System.Collections.Generic;

namespace Web.Models
{
    class DogWalkingProfessional
    {
        public int Id { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public PriceRate PriceRate { get; set; }
        public IEnumerable<Walk> Walks { get; set; }
        public DailySchedule GetDailySchedule { get; set; }
    }

    class Customer
    {
        public int Id { get; set; }
        public IEnumerable<Dog> Dogs { get; set; }
    }

    class PriceRate
    {
        public DogSize DogSize { get; set; }
        public bool IsAggressive { get; set; }
        public decimal PricePerWalk { get; set; }
        public byte MaxInPack { get; set; }
        public TimeSpan WalkDuration { get; set; }
    }

    class Dog 
    {
        public int Id { get; set; }
        public bool IsAggressive { get; set; }
        public DogSize Size { get; set; }
        public Customer Owner { get; set; }
    }

    class DogPack
    {
        public IEnumerable<Dog> Dogs { get; set; }
    }
    
    class Schedule
    {
        public IEnumerable<Walk> Walks { get; set; }
    }

    class DailySchedule
    {
        public DayOfWeek DayOfWeek { get; set; }
        public IEnumerable<TimeFrame> TimeFrames { get; set; }
    }

    class TimeFrame
    {
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

    class Walk
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsDone { get; set; }
        public DogPack DogPack { get; set; }
    }

    enum DogSize
    {
        None = 0,
        Small = 1,
        //Medium = 2, 
        Large = 3
    }

    // InMemory store
    // Separate libs
    // Separate files
    // Is there any chanse we will need to work with cats for example?
}
