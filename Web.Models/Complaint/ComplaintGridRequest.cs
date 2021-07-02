using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Complaint
{
    [ModelBinder(typeof(AjaxRequestModelBinder<ComplaintGridRequest>))]
    public class ComplaintGridRequest : AjaxRequestModel<ComplaintGridRequest>
    {
        public string EmployeeName { get; set; }
        public string PatientName { get; set; }
        public string DateReported { get; set; }
        public string DateOccurred { get; set; }
        public string Wing { get; set; }
        public string ReportedBy { get; set; }
        public string ComplaintTypeName { get; set; }

        public Expression<Func<Domain.Models.Complaint, object>> SortBy 
        {
            get
            {
                return i => i.Id;
            }
        }
    }
}
