using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class DogWalker
    {
        public int Id { get; set; }
        public List<Customer> Customers { get; set; }
        public PriceRate PriceRate { get; set; }
        public List<WorkingDay> WorkingDays { get; set; }
        public List<Walk> BookedWalks { get; set; }
        public List<DogPack> DogPacks { get; set; }
        public TimeSpan DefaultWalkDuration { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public List<Dog> Dogs { get; set; }
    }

    public class PriceRate
    {
        public DogSize DogSize { get; set; }
        public bool IsAggressive { get; set; }
        public decimal PricePerWalk { get; set; }
        public byte MaxInPack { get; set; }
        public TimeSpan WalkDuration { get; set; }
    }

    public class Dog
    {
        public int Id { get; set; }
        public bool IsAggressive { get; set; }
        public DogSize Size { get; set; }
    }

    public class DogPack
    {
        public List<Dog> Dogs { get; set; }
    }

    public class WorkingDay
    {
        public DayOfWeek DayOfWeek { get; set; }
        public List<TimeSpan> WorkingHours { get; set; }
    }   

    public class Walk
    {
        public DateTime StartDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        //public bool IsDone { get; set; }
        public DogPack DogPack { get; set; }
    }

    public enum DogSize
    {
        None = 0, // TODO: consider to delete this
        Small = 1,
        Large = 3
    }

    public class DogPackConfig
    {
        public DogSize DogSize { get; set; }
        public bool IsAggressive { get; set; }
        public byte MaxPackCount { get; set; }
    }

    public class RevenuReport
    {
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }

    public class WeeklySchedulePayload
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime Now { get; set; }
        public List<WorkingDay> WorkingDays { get; set; }
        public TimeSpan DayStartsAt { get; set; }
        public TimeSpan DayEndsAt { get; set; }
        public List<DogPack> DogPacks { get; set; }
        public TimeSpan WalkDuration { get; set; }
    }

    // customers schedule
    // InMemory store
    // Separate libs
    // Separate files
    // Is there any chanse we will need to work with cats for example?
    // if there are more than two dogPacks
}
