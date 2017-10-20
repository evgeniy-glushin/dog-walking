using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Services.Abstraction;
using static Web.Services.Imptemantation.DatesHelper;

namespace Web.Services
{
    public class WeekDaysScheduleBuilder : IScheduleBuilder
    {
        /// <summary>
        /// Creates walks on the week days. Calculates how much each walk costs according to the rates provided.
        /// </summary>
        public IEnumerable<Walk> Build(WeekDaysSchedulePayload payload)
        {
            var packsWithDogs = payload?.DogPacks?.Where(dp => dp.Dogs?.Any() ?? false).ToList();

            if (!packsWithDogs?.Any() ?? true || payload?.WorkingDays == null)
                return Enumerable.Empty<Walk>();

            var getDogPack = BuildDogPackFetcher();            
            
            return Range(payload.DateFrom, payload.DateTo)
                .Where(d => d.IsWeekDay())
                .SelectMany(date => CreateWalks(date, getDogPack, payload));                      
           
            // Returns function which gets dog packs one by one in turn
            Func<DogPack> BuildDogPackFetcher()
            {
                var dogPacksQueue = new Queue<DogPack>();
                return () =>
                {
                    if (!dogPacksQueue.Any())
                        dogPacksQueue = new Queue<DogPack>(packsWithDogs);

                    return dogPacksQueue.Dequeue();
                };
            }
        }

        IEnumerable<Walk> CreateWalks(DateTime date, Func<DogPack> getDogPack, WeekDaysSchedulePayload payload)
        {
            return payload.WorkingDays
               .FirstOrDefault(ds => ds.DayOfWeek == date.DayOfWeek)
               ?.WorkingHours
               .Where(InTimeBoundaries)
               .Select(CreateWalk) ?? Enumerable.Empty<Walk>();

            Walk CreateWalk(TimeSpan time)
            {
                var pack = getDogPack();
                return new Walk
                {
                    StartDateTime = date.Date + time,
                    DogPack = pack,
                    Duration = payload.WalkDuration,
                    Price = CalcPrice(pack)
                };
            }

            decimal CalcPrice(DogPack pack)
            {
                Dog firstDog = pack.Dogs.First();
                var (size, isAggressive) = (firstDog.Size, firstDog.IsAggressive);

                var rate = payload.PriceRates
                    .FirstOrDefault(pr => pr.DogSize == size && 
                                          pr.IsAggressive == isAggressive);

                return pack.Dogs.Count * rate.PricePerWalk;
            }

            bool InTimeBoundaries(TimeSpan wh) =>
                date.Date == payload.Now.Date ?
                    wh >= payload.Now.TimeOfDay && payload.Now.TimeOfDay <= wh : true;
        }        
    }
}
