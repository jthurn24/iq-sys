using System;
using System.Collections.Generic;
using System.Linq;


namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class IncidentTypeGroup : BaseReportingEntity
    {
        public virtual string Name { get; set; }
        public virtual string Color { get; set; }
    }
}
