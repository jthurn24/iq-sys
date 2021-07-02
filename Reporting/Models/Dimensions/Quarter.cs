using System;
using System.Collections.Generic;
using System.Linq;


namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class Quarter : BaseReportingEntity
    {
        public virtual int QuarterOfYear { get; set; }
        public virtual int Year { get; set; }

    }
}
