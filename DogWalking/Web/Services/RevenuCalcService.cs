using System;
using System.Collections.Generic;
using Web.Models;

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
