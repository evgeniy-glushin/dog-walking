using Domain.Models;
using System;
using System.Collections.Generic;
using static Web.Services.Imptemantation.DatesHelper;

namespace Web.Services
{
    class RevenuCalcService
    {
        public IEnumerable<RevenuReport> Calc(DateTime dateFrom, 
            DateTime dateTo, 
            IEnumerable<Walk> bookedWalks)
        {
            throw new NotImplementedException();
        }
    }
}
