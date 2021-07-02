using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;


namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class FacilityInfectionSite : BaseReportingEntity
    {
        public virtual Facility Facility { get; set; }
        public virtual IList<Entry> Entries { get; set; }

        public class Entry
        {
            public virtual InfectionType InfectionType { get; set; }
            public virtual InfectionSite InfectionSite { get; set; }
        }
    }
}
