using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Cubes.Virtual
{
    public class FacilityGroupMonthInfectionType
    {
        public virtual InfectionType InfectionType { get; set; }
        public virtual int Total { get; set; }
        public virtual Decimal Rate { get; set; }
        public virtual Month Month { get; set; }
        public virtual Decimal Change { get; set; }
        public virtual int NonNosoTotal { get; set; }
    }
}
