using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.CatheterAssessment
{
    [ModelBinder(typeof(AjaxRequestModelBinder<AssessmentGridRequest>))]
    public class AssessmentGridRequest : AjaxRequestModel<AssessmentGridRequest>
    {
        public DateTime? AssessmentDate { get; set; }

        public Expression<Func<Domain.Models.CatheterAssessment, object>> SortBy
        {
            get
            {

                if (RequestedSortBy(model => model.AssessmentDate))
                {
                    return x => x.AssessmentDate;
                }

                return i => i.Id;
            }
        }
    }
}
