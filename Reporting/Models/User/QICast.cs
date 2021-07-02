using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Reporting.Models.User
{
    public class QICast : BaseReportingEntity
    {
        public IList<string> Dashboards { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public Guid FacilityId { get; set; }
        public bool Inactive { get; set; }
    }
}
