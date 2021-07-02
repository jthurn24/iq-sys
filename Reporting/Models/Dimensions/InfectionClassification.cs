using System;
using System.Collections.Generic;
using System.Linq;

namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class InfectionClassification : BaseReportingEntity
    {
        public virtual string EnumName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsNosocomial { get; set; }
        public virtual bool IsQualified { get; set; }
    }
}
