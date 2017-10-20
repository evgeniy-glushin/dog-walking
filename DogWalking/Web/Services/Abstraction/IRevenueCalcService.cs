using Domain.Models;
using System.Collections.Generic;

namespace Web.Services.Abstraction
{
    public interface IRevenueCalcService
    {
        IEnumerable<RevenueReport> Calc(IEnumerable<Walk> bookedWalks);
    }
}
