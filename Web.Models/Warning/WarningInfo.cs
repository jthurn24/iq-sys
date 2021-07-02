using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Warning
{
    public class WarningInfo
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string TriggeredOn { get; set; }

        public string Target { get; set; }

        public string PatientName { get; set; }

        public string FacilityName { get; set; }

        public string DescriptionText { get; set; }

        public bool Recent { get; set; }
    }
}
