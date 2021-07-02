using System;
using System.Collections.Generic;
using System.Linq;
using IQI.Intuition.Reporting.Models.Dimensions;


namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class AverageTypeMonthInfectionType : BaseReportingEntity
    {
        public virtual InfectionType InfectionType { get; set; }
        public virtual AverageType AverageType { get; set; }
        public virtual int Total { get; set; }
        public virtual Decimal Rate { get; set; }
        public virtual Month Month { get; set; }
        public virtual DateTime? CalculatedOn { get; set; }
        public virtual int TotalFacilities { get; set; }
        public virtual int CensusPatientDays { get; set; }
    }
}
