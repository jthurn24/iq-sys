using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.PsychotropicDosageChange
{
    [ModelBinder(typeof(AjaxRequestModelBinder<AdministrationPsychotropicDosageGridRequest>))]
    public class AdministrationPsychotropicDosageGridRequest : AjaxRequestModel<AdministrationPsychotropicDosageGridRequest>
    {
        public string Id { get; set; }
        public int? AdministrationId { get; set; }
        public string Dosage { get; set; }
        public string StartDate { get; set; }
        public string Frequency { get; set; }

        public Expression<Func<Domain.Models.PsychotropicDosageChange, object>> SortBy
        {
            get
            {

                if (RequestedSortBy(model => model.StartDate))
                {
                    return x => x.StartDate;
                }

                if (RequestedSortBy(model => model.Frequency))
                {
                    return i => i.Frequency;
                }

                return i => i.Id;
            }
        }
    }
}
