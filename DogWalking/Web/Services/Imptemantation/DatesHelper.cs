using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Services.Imptemantation
{
    public static class DatesHelper
    {
        /// <summary>
        /// Generates a sequence of dates within specified range
        /// </summary>
        public static IEnumerable<DateTime> Range(DateTime fromDate, DateTime toDate) =>
            fromDate <= toDate ? Enumerable.Range(0, toDate.Subtract(fromDate).Days + 1)
            .Select(d => fromDate.AddDays(d)) : Enumerable.Empty<DateTime>();

        public static bool IsWeekDay(this DateTime dateTime) =>
            dateTime.DayOfWeek != DayOfWeek.Saturday && 
            dateTime.DayOfWeek != DayOfWeek.Sunday;
    }
}
