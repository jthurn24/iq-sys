using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class LineListingLabResultView
    {
        public string TestType { get; set; }
        public string CompletedOn { get; set; }
        public string Result { get; set; }
        public string Notes { get; set; }
        public List<string> Pathogens { get; set; }
    }
}
