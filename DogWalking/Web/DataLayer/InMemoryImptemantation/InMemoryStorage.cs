using Domain.Models;
using System;
using System.Collections.Generic;

namespace Web.DataLayer.InMemoryImptemantation
{
    static class InMemoryStorage
    {
        public static IEnumerable<DogWalker> DogWalkers;
        static InMemoryStorage()
        {
            DogWalkers = new List<DogWalker>()
            {
                new DogWalker
                {
                    Id = 1,
                    WorkingDays = new List<WorkingDay>
                    {
                        new WorkingDay { DayOfWeek = DayOfWeek.Monday, WorkingHours = new List<TimeSpan> { TimeSpan.FromHours(10), TimeSpan.FromHours(16) } },
                        new WorkingDay { DayOfWeek = DayOfWeek.Tuesday, WorkingHours = new List<TimeSpan> { TimeSpan.FromHours(10), TimeSpan.FromHours(16) } },
                        new WorkingDay { DayOfWeek = DayOfWeek.Wednesday, WorkingHours = new List<TimeSpan> { TimeSpan.FromHours(10), TimeSpan.FromHours(16) } },
                        new WorkingDay { DayOfWeek = DayOfWeek.Thursday, WorkingHours = new List<TimeSpan> { TimeSpan.FromHours(10), TimeSpan.FromHours(16) } },
                        new WorkingDay { DayOfWeek = DayOfWeek.Friday, WorkingHours = new List<TimeSpan> { TimeSpan.FromHours(10), TimeSpan.FromHours(16) } },
                    },
                    Customers = new List<Customer>
                    {
                        new Customer
                        {
                            Id = 1,
                            Dogs = new List<Dog>
                            {
                                new Dog { Name = "Customer1dog1", IsAggressive = false, Size = DogSize.Small },
                                new Dog { Name = "Customer1dog2", IsAggressive = false, Size = DogSize.Large },
                                new Dog { Name = "Customer1dog3", IsAggressive = false, Size = DogSize.Small },
                            }
                        },
                        new Customer
                        {
                            Id = 2,
                            Dogs = new List<Dog>
                            {
                                new Dog { Name = "Customer2dog1", IsAggressive = true, Size = DogSize.Small }
                            }
                        },
                        new Customer
                        {
                            Id = 3,
                            Dogs = new List<Dog>
                            {
                                new Dog { Name = "Customer3dog1", IsAggressive = true, Size = DogSize.Small }
                            }
                        },
                        new Customer
                        {
                            Id = 4,
                            Dogs = new List<Dog>
                            {
                                new Dog { Name = "Customer4dog1", IsAggressive = false, Size = DogSize.Small }
                            }
                        },
                        new Customer
                        {
                            Id = 5,
                            Dogs = new List<Dog>
                            {
                                new Dog { Name = "Customer5dog1", IsAggressive = false, Size = DogSize.Small },
                                new Dog { Name = "Customer5dog2", IsAggressive = false, Size = DogSize.Small },
                                new Dog { Name = "Customer5dog3", IsAggressive = false, Size = DogSize.Small }
                            }
                        }
                    },
                    DefaultWalkDuration = TimeSpan.FromHours(1),
                    PriceRates = new List<PriceRate>
                    {
                        new PriceRate { DogSize = DogSize.Small, IsAggressive = false, MaxPacksCount = 5, PricePerWalk = 10 },
                        new PriceRate { DogSize = DogSize.Large, IsAggressive = false, MaxPacksCount = 3, PricePerWalk = 20 },
                        new PriceRate { DogSize = DogSize.Small, IsAggressive = true, MaxPacksCount = 1, PricePerWalk = 55 },
                        new PriceRate { DogSize = DogSize.Large, IsAggressive = true, MaxPacksCount = 1, PricePerWalk = 65 }
                    }
                }
            };
        }
    }
}
