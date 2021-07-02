using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Facts
{
    public class WoundReport : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual WoundClassification Classification { get; set; }
        public virtual Month FirstNotedOnMonth { get; set; }
        public virtual Quarter FirstNotedOnQuarter { get; set; }
        public virtual Month ClosedOnMonth { get; set; }
        public virtual Quarter ClosedOnQuarter { get; set; }
        public virtual DateTime? FirstNotedOnDate { get; set; }
        public virtual DateTime? ClosedOnDate { get; set; }
        public virtual WoundSite Site { get; set; }
        public virtual WoundType WoundType { get; set; }
        public virtual List<Assessment> Assessments { get; set; }


        public class Assessment
        {
            public virtual WoundStage Stage { get; set; }
            public virtual Month Month { get; set; }
            public virtual Quarter Quarter { get; set; }
            public virtual DateTime? AssessmentDate { get; set; }
            public virtual FloorMap FloorMap { get; set; }
            public virtual FloorMapRoom FloorMapRoom { get; set; }
            public virtual Room Room { get; set; }
            public virtual Wing Wing { get; set; }
            public virtual Floor Floor { get; set; }
            public virtual Day Day { get; set; }
            public virtual DateTime? CoverageEndDate { get; set; }


        }

    }
}
