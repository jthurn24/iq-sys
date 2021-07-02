using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Domain
{
    public static class Calculations
    {
        public static decimal Rate1000(int total, int days)
        {
            if (total > 0 && days > 0)
            {
                return ((total / Convert.ToDecimal(days)) * 1000);
            }

            return 0;
        }  

        public static decimal RateChange(decimal prevRate, decimal newRate)
        {
            return 0 - (prevRate - newRate);
        }
    }
}
