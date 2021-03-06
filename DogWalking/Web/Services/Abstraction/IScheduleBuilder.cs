﻿using Domain.Models;
using System.Collections.Generic;

namespace Web.Services.Abstraction
{
    public interface IScheduleBuilder
    {
        IEnumerable<Walk> Build(WeekDaysSchedulePayload payload);
    }
}
