using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.WoundAssessment
{
    [ModelBinder(typeof(AjaxRequestModelBinder<AssessmentGridRequest>))]
    public class AssessmentGridRequest : AjaxRequestModel<AssessmentGridRequest>
    {
        public int? Stage { get; set; }
        public DateTime? AssessmentDate { get; set; }

        public Expression<Func<Domain.Models.WoundAssessment, object>> SortBy
        {
            get
            {

                if (RequestedSortBy(model => model.Stage))
                {
                    return x => x.Stage.RatingValue;
                }

                if (RequestedSortBy(model => model.AssessmentDate))
                {
                    return x => x.AssessmentDate;
                }

              

                return i => i.Id;
            }
        }
    }
}
