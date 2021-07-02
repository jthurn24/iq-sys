using System;
using System.Collections.Generic;
using System.Linq;


namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class FloorMap : BaseReportingEntity
    {
        public virtual string Name { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual Wing Wing { get; set; }
        public virtual bool Active { get; set; }

    }
}
