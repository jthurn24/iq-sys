using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;


namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class FacilityMonthCensus : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual Decimal Average { get; set; }
        public virtual int TotalPatientDays { get; set; }
        public virtual int TotalDays { get; set; }
        public virtual Month Month { get; set; }
    }
}
