using Domain.Models;
using System;
using System.Collections.Generic;

namespace Web.Services
{
    class RevenuCalcService
    {
        public IEnumerable<RevenuReport> Calc(DateTime dateFrom, DateTime dateTo, IEnumerable<Walk> bookedWalks, PriceRate priceRate)
        {
            throw new NotImplementedException();
        }
    }
}
