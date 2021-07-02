using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace IQI.Intuition.Web.Models.Complaint
{
    public class ComplaintForm
    {
        public int? Id { get; set; } 
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int? PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime? DateReported { get; set; }
        public DateTime? DateOccurred { get; set; }
        public int? Wing { get; set; }
        public string ReportedBy { get; set; }
        public string DescriptionText { get; set; }
        public int? ComplaintType { get; set; }
        public IEnumerable ComplaintTypeDescriptions { get; set; }
        public bool? Reported { get; set; }
        public bool? Cleared { get; set; }

        public int? Employee2Id { get; set; }
        public string Employee2Name { get; set; }

        public int? Patient2Id { get; set; }
        public string Patient2Name { get; set; }
    }
}
