using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class FacilityMonthIncidentInjury : BaseReportingEntity
    {

        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual IList<Entry> Entries { get; set; }

        public class Entry : AnnotatedEntry
        {
            public virtual int Total { get; set; }
            public virtual Decimal Rate { get; set; }
            public virtual Month Month { get; set; }
            public virtual Decimal Change { get; set; }
            public virtual IncidentTypeGroup IncidentTypeGroup { get; set; }
            public virtual int CensusPatientDays { get; set; }
            public virtual IncidentInjury IncidentInjury { get; set; }
        }

    }
}
