using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Services.Abstraction;
using static Web.Services.Imptemantation.DatesHelper;

namespace Web.Services
{
    public class RevenueCalcService : IRevenueCalcService
    {
        /// <summary>
        /// Generates revenue report in the date range between the earliest and the latest walk.
        /// </summary>
        public IEnumerable<RevenueReport> Calc(IEnumerable<Walk> bookedWalks)
        {
            if (bookedWalks == null || bookedWalks.Count() == 0)
                return Enumerable.Empty<RevenueReport>();

            var (dateFrom, dateTo) = (bookedWalks.Min(w => w.StartDateTime.Date),
                                      bookedWalks.Max(w => w.StartDateTime.Date));

            return Range(dateFrom, dateTo)
                .Select(CalcTotal);

            RevenueReport CalcTotal(DateTime date)
            {
                var sum = bookedWalks.Where(w => w.StartDateTime.Date == date.Date)
                                     .Sum(w => w.Price);
                return new RevenueReport { Date = date, Total = sum };
            }
        }
    }
}
