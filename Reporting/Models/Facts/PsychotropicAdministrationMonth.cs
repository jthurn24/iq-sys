using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Facts
{
    public class PsychotropicAdministrationMonth : BaseReportingEntity
    {
        public virtual Month Month { get; set; }
        public virtual int? TotalDays { get; set; }
        public virtual decimal? TotalDosage { get; set; }

    }
}
