using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class InfectionSite : BaseReportingEntity
    {
        public virtual string Name { get; set; }
        public virtual InfectionType InfectionType { get; set; }
    }
}
