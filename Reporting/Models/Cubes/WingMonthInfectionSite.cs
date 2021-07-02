using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class WingMonthInfectionSite : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual IList<InfectionSiteEntry> InfectionSiteEntries { get; set; }
        

        public class InfectionSiteEntry
        {
            public virtual InfectionType InfectionType { get; set; }
            public virtual List<WingEntry> WingEntries { get; set; }
            public virtual InfectionSite InfectionSite { get; set; }
        }

        public class WingEntry
        {
            public virtual Wing Wing { get; set; }
            public virtual List<Entry> Entries { get; set;  }
        }

        public class Entry : AnnotatedEntry
        {
        
            public virtual int Total { get; set; }
            public virtual Decimal Rate { get; set; }
            public virtual Month Month { get; set; }
            public virtual Decimal Change { get; set; }        
        }
    }
}
