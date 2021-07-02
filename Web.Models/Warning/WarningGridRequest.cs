using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Warning
{
    [ModelBinder(typeof(AjaxRequestModelBinder<WarningGridRequest>))]
    public class WarningGridRequest : AjaxRequestModel<WarningGridRequest>
    {

        public string Title { get; set; }
        public string TriggeredOn { get; set; }
        public string PatientName { get; set; }


        public Expression<Func<Domain.Models.Warning, object>> SortBy 
        {
            get
            {
                if (RequestedSortBy(model => model.TriggeredOn))
                {
                    return i => i.TriggeredOn;
                }
                else if (RequestedSortBy(model => model.Title))
                {
                    return i => i.Title;
                }
                else if (RequestedSortBy(model => model.Title))
                {
                    return i => i.Patient.FullName;
                }

                return i => i.TriggeredOn;
            }
        }
    }
}
