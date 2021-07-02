using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class Month : BaseReportingEntity
    {
        public virtual int MonthOfYear { get; set; }
        public virtual int Year { get; set; }
        public virtual Quarter Quarter { get; set; }
        public virtual string Name { get; set; }
    }
}
