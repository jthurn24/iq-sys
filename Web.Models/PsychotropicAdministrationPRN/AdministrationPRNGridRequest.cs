using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;

namespace IQI.Intuition.Web.Models.PsychotropicAdministrationPRN
{
    [ModelBinder(typeof(AjaxRequestModelBinder<AdministrationPsychotropicDosageGridRequest>))]
    public class AdministrationPsychotropicDosageGridRequest : AjaxRequestModel<AdministrationPsychotropicDosageGridRequest>
    {
        public string Id { get; set; }
        public int? AdministrationId { get; set; }
        public string Dosage { get; set; }
        public string GivenOn { get; set; }

        public Expression<Func<Domain.Models.PsychotropicAdministrationPRN, object>> SortBy
        {
            get
            {
                if (RequestedSortBy(model => model.Dosage))
                {
                    return x => x.Dosage;
                }

                if (RequestedSortBy(model => model.Dosage))
                {
                    return x => x.Dosage;
                }

                if (RequestedSortBy(model => model.GivenOn))
                {
                    return i => i.GivenOn;
                }

                return i => i.Id;
            }
        }
    }
}
