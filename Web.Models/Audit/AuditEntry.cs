using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Audit
{
    public class AuditEntry
    {
        public string PerformedAt { get; set; }
        public string PerformedBy { get; set; }
        public string IPAddress { get; set; }
        public string Details { get; set; }
        public string AuditType { get; set; }
        public string TargetPatient { get; set; }
        public string Description { get; set; }
        public string PatientName { get; set; }


    }
}
