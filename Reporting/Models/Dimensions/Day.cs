using System;
using System.Collections.Generic;
using System.Linq;


namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class Day : BaseReportingEntity
    {
        public virtual int DayOfMonth{ get; set; }
        public virtual int MonthOfYear { get; set; }
        public virtual int Year { get; set; }

        public virtual DateTime GetDateTime()
        {
               return new DateTime(Year, MonthOfYear, DayOfMonth);
        }
    }
}
