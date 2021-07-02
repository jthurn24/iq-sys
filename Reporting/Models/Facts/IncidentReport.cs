using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;


namespace IQI.Intuition.Reporting.Models.Facts
{
    public class IncidentReport : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual Room Room { get; set; }
        public virtual FloorMap FloorMap { get; set; }
        public virtual FloorMapRoom FloorMapRoom { get; set; }
        public virtual Floor Floor { get; set; }
        public virtual Wing Wing { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual List<IncidentType> IncidentTypes { get; set; }
        public virtual List<IncidentTypeGroup> IncidentTypeGroups { get; set; }
        public virtual Day Day { get; set; }
        public virtual Month Month { get; set; }
        public virtual Quarter Quarter { get; set; }
        public virtual DateTime? OccurredOnDate { get; set; }
        public virtual IncidentLocation IncidentLocation { get; set; }
        public virtual List<IncidentInjury> IncidentInjuries { get; set; }
        public virtual IncidentInjuryLevel IncidentInjuryLevel { get; set; }
        public virtual int? OccurredOnDayOfWeek { get; set; }
        public virtual int? OccurredOnHourOfDay { get; set; }
        public virtual int? DiscoveredOnDayOfWeek { get; set; }
        public virtual int? DiscoveredOnHourOfDay { get; set; }
        public virtual DateTime? DiscoveredOnDate { get; set; }

    }


}
