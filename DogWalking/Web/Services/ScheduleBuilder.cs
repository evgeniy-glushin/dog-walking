using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models;

namespace Web.Services
{
    public class WeeklyScheduleBuilder
    {
        public static TimeSpan DayStartsAt => TimeSpan.FromDays(10);
        public static TimeSpan DayEndsAt => TimeSpan.FromDays(16);

        public IEnumerable<Walk> Build(WeeklySchedulePayload payload)
        {
            var packsWithDogs = payload?.DogPacks?.Where(dp => dp.Dogs?.Any() ?? false);
            if (!packsWithDogs?.Any() ?? false || payload?.WorkingDays == null)
                return Enumerable.Empty<Walk>();
            
            return DateRange(payload.DateFrom, payload.DateTo)
                .Where(d => IsWeekDay(d.DayOfWeek))
                .Select(d => CreateWolks(d, packsWithDogs, payload))
                .SelectMany(x => x);

            IEnumerable<DateTime> DateRange(DateTime fromDate, DateTime toDate) =>
                Enumerable.Range(0, toDate.Subtract(fromDate).Days + 1)
                          .Select(d => fromDate.AddDays(d));

            bool IsWeekDay(DayOfWeek dayOfWeek) =>
                dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
        }

        IEnumerable<Walk> CreateWolks(DateTime date, IEnumerable<DogPack> packsWithDogs, WeeklySchedulePayload payload)
        {
            var getDogPack = BuildDogPackFetcher();

            return payload.WorkingDays
               .FirstOrDefault(ds => ds.DayOfWeek == date.DayOfWeek)
               ?.WorkingHours
               .Where(TimeFilter)
               .Select(wh => new Walk
               {
                   StartDateTime = date.Date + wh,
                   DogPack = getDogPack(),
                   Duration = payload.WalkDuration
               }) ?? Enumerable.Empty<Walk>();


            Func<DogPack> BuildDogPackFetcher()
            {
                var dogPacksQueue = new Stack<DogPack>();
                return () =>
                {
                    if (!dogPacksQueue.Any())
                        dogPacksQueue = new Stack<DogPack>(packsWithDogs);

                    return dogPacksQueue.Pop();
                };
            }

            bool TimeFilter(TimeSpan wh)
            {
                var isRightDayTime = wh >= payload.DayStartsAt &&
                                     wh <= payload.DayEndsAt - payload.WalkDuration;

                if (date.Date == payload.Now.Date)
                    isRightDayTime = isRightDayTime &&
                        wh >= payload.DateFrom.TimeOfDay &&
                        wh <= payload.DateTo.TimeOfDay;

                return isRightDayTime;
            }
        }       
    }
}
