using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;


namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class FacilityMonthCatheter : BaseReportingEntity
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
            public virtual int DeviceDays { get; set; }
            public virtual Decimal UtilizationRatio { get; set; }
            public virtual CatheterType CatheterType { get; set; }
        }


    }
}
