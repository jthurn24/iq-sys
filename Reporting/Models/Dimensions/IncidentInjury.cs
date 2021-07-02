using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class IncidentInjury : BaseReportingEntity
    {
        public virtual string Name { get; set; }
        public virtual int? SortOrder { get; set;}
        public virtual string Color { get; set; }
    }
}
