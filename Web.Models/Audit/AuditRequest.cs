using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Audit
{
    public class AuditRequest
    {
        public Guid? PatientId { get; set; }
        public Guid? ComponentId { get; set; }
        public IList<Domain.Enumerations.AuditEntryType> Types { get; set;}
        public int Page { get; set; }
        public string AuditDescription { get; set; }
    }
}
