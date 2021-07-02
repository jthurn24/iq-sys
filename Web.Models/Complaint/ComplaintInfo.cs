using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Complaint
{
    public class ComplaintInfo
    {
        public string Id { get; set; }
        public string EmployeeName { get; set; }
        public string PatientName { get; set; }
        public string DateReported { get; set; }
        public string DateOccurred { get; set; }
        public string Wing { get; set  ; }
        public string ReportedBy { get; set; }
        public string ComplaintTypeName { get; set; }
    }
}
 