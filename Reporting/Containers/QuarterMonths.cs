using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Containers
{
    public class QuarterMonths
    {
        public Quarter Quarter { get; set; }
        public Month Month1 { get; set; }
        public Month Month2 { get; set; }
        public Month Month3 { get; set; }
    }
}
