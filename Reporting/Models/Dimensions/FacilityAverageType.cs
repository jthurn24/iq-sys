using System;
using System.Collections.Generic;
using System.Linq;


namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class FacilityAverageType : BaseReportingEntity
    {
        public virtual AverageType AverageType { get; set; }
        public virtual Facility Facility { get; set; }
    }
}
