using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.PatientCensus
{
    [ModelBinder(typeof(AjaxRequestModelBinder<PatientCensusGridRequest>))]
    public class PatientCensusGridRequest : AjaxRequestModel<PatientCensusGridRequest>
    {
        public int Id { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int PatientDays { get; set; }

        public Expression<Func<Domain.Models.PatientCensus, object>> SortBy 
        {
            get
            {
                return i => i.Id;
            }
        }
    }
}
