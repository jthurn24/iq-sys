using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.Incident
{
    public class RepeatFallCalculator
    {
        public decimal AveragePerMonth(DateTime startDate,
            DateTime? endDate,
            IEnumerable<IncidentReport> reports)
        {
            decimal retVal = 0;

            if (reports.Count() < 1)
            {
                return retVal;
            }


            if (endDate.HasValue == false)
            {
                endDate = DateTime.Today;
            }

            int totalMonths = ((endDate.Value.Year - startDate.Year) * 12) + endDate.Value.Month - startDate.Month;

            if (totalMonths < 1)
            {
                return 0;
            }

            retVal = (decimal)reports.Count() / (decimal)totalMonths;

            return retVal;
        }
    }
}
