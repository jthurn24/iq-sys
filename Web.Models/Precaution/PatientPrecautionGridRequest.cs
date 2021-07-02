using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.Precaution
{
    [ModelBinder(typeof(AjaxRequestModelBinder<PatientPrecautionGridRequest>))]
    public class PatientPrecautionGridRequest : AjaxRequestModel<PatientPrecautionGridRequest>
    {

        public Guid? PatientGuid { get; set; }
        public int? ProductId { get; set; }
        public int Id { get; set; }
        public string PrecautionTypeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public Expression<Func<Domain.Models.PatientPrecaution, object>> SortBy 
        {
            get
            {

                if (RequestedSortBy(model => model.PrecautionTypeName))
                {
                    return p => p.PrecautionType.Name;
                }


                if (RequestedSortBy(model => model.StartDate))
                {
                    return p => p.StartDate;
                }


                if (RequestedSortBy(model => model.EndDate))
                {
                    return p => p.EndDate;
                }

                return i => i.Id;
            }
        }
    }
}
