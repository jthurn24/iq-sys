﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class FacilityMonthWoundClassification : BaseReportingEntity 
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
            public virtual Decimal PercentageOfPopulation { get; set; }
            public virtual Dimensions.WoundClassification Classification { get; set; }
            public virtual Dimensions.WoundType WoundType { get; set; }

        }

    }
}
