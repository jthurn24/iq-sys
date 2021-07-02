using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Facts
{
    public class Complaint : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual Floor Floor { get; set; }
        public virtual Wing Wing { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual Month Month { get; set; }
        public virtual Quarter Quarter { get; set; }
        public virtual DateTime? OccurredDate { get; set; }
        public virtual ComplaintType ComplaintType { get; set; }
     
    }
}
