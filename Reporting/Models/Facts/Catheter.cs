using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Facts
{
    public class Catheter : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual DateTime? StartedDate { get; set; }
        public virtual DateTime? DiscontinuedDate { get; set; }
        public virtual CatheterType CatheterType { get; set; }
    }
}
