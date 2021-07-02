using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Wound
{
    public class ActiveWoundMapDetail
    {
        public string SiteName { get; set; }
        public IList<Wound> Wounds { get; set; }

        public class Wound
        {
            public string PatientName { get; set; }
            public string PatientGuid { get; set; }
            public string Stage { get; set; }
        }
    }
}
