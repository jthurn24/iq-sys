using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IQI.Intuition.Reporting.Models.Dimensions;
namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class FacilityMonthPsychotropicDrugType : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual IList<Entry> Entries { get; set; }

        public class Entry : AnnotatedEntry
        {
            public virtual Month Month { get; set; }
            public virtual int? IncreaseCount { get; set; }
            public virtual int? DecreaseCount { get; set; }
            public virtual int? ActiveCount { get; set; }
            public virtual decimal? ActiveRate { get; set; }
            public virtual int? ActiveChange { get; set; }
            public virtual int? DosageChange { get; set; }
            public virtual PsychotropicDrugType DrugType { get; set; }
        }

    }
}
