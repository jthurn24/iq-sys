using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Facts
{
    public class InfectionVerification : BaseReportingEntity
    {
        public virtual InfectionType InfectionType { get; set; }
        public virtual InfectionClassification InfectionClassification { get; set; }
        public virtual InfectionSite InfectionSite { get; set; }

        public virtual Account Account {get; set; }
        public virtual Facility Facility { get; set; }
        public virtual Room Room { get; set; }
        public virtual Wing Wing { get; set; }
        public virtual Floor Floor { get; set; }


        public virtual FloorMap FloorMap { get; set; }
        public virtual FloorMapRoom FloorMapRoom { get; set; }

        public virtual Month NotedOnMonth { get; set; }
        public virtual Quarter NotedOnQuarter { get; set; }
        public virtual Day NotedOnDay { get; set; }
        public virtual DateTime? NotedOnDate { get; set; }
        public virtual Month ClosedOnMonth { get; set; }
        public virtual Quarter ClosedOnQuarter { get; set; }
        public virtual Day ClosedOnDay { get; set; }
        public virtual DateTime? ClosedOnDate { get; set; }
        public virtual string SupportingDetail { get; set; }

        public virtual bool? Deleted { get; set; }



    }
}
