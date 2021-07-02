using System;
using System.Collections.Generic;
using System.Linq;



namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class Floor : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual string Name { get; set; }
    }
}
