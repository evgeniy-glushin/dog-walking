using Domain.Models;
using System.Collections.Generic;

namespace Web.Services
{
    public interface IWalksBuilder
    {
        IEnumerable<Walk> Build(WeeklySchedulePayload payload);
    }
}
