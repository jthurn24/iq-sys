using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;

namespace IQI.Intuition.Web.Models.Reporting.Complaint.Facility
{
    public class LineListingComplaintView
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ComplaintType { get; set; }
        public IEnumerable<SelectListItem> ComplaintTypeOptions { get; set; }
        public int? Employee { get; set; }
        public IEnumerable<SelectListItem> EmployeeOptions { get; set; }
        public int? Wing { get; set; }
        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? SortBy { get; set; }
        public IEnumerable<SelectListItem> SortOptions { get; set; }


        public IList<ComplaintRow> Complaints { get; set; }

        public void SetData(IEnumerable<Domain.Models.Complaint> complaintData)
        {
            this.Complaints = new List<ComplaintRow>();

            foreach (var complaint in complaintData)
            {
                var row = new ComplaintRow();
                row.Id = complaint.Id.ToString();
                row.PatientName = complaint.Patient != null ? complaint.Patient.FullName : string.Empty;
                row.PatientName2 = complaint.Patient2 != null ? complaint.Patient2.FullName : string.Empty;
                row.EmployeeName = complaint.Employee != null ? complaint.Employee.FullName : string.Empty;
                row.EmployeeName2 = complaint.Employee2 != null ? complaint.Employee2.FullName : string.Empty;
                row.DateOccurred = complaint.DateOccurred.FormatAsShortDate();
                row.DateReported = complaint.DateReported.FormatAsShortDate();
                row.Wing = complaint.Wing != null ? complaint.Wing.Name : string.Empty;
                row.ReportedBy = complaint.ReportedBy;
                row.ComplaintType = complaint.ComplaintType.Name;
                row.Description = complaint.DescriptionText;
                row.Reported = complaint.Reported.FormatAsAnswer();
                row.Cleared = complaint.Cleared.FormatAsAnswer();
                this.Complaints.Add(row);
            }
        }

        public class ComplaintRow
        {
            public string Id { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeName2 { get; set; }
            public string PatientName { get; set; }
            public string PatientName2 { get; set; }
            public string DateReported { get; set; }
            public string DateOccurred { get; set; }
            public string Wing { get; set; }
            public string ReportedBy { get; set; }
            public string ComplaintType { get; set; }
            public string Description { get; set; }
            public string Reported { get; set; }
            public string Cleared { get; set; }
        }
    }
}
