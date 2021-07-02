using System;
using System.Collections.Generic;
using System.Linq;


namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class IncidentType : BaseReportingEntity
    {
        public virtual string Name { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual IncidentTypeGroup IncidentTypeGroup { get; set; }
        public virtual string Color { get; set; }
    }
}
