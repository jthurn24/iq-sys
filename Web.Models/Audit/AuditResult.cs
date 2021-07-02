using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Audit
{
    public class AuditResult
    {
        public AuditRequest Request { get; set; }
        public IEnumerable<AuditEntry> Results { get; set; }
        public int TotalPages { get; set; }
    }
}
