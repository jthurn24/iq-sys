using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PsychotropicAdministrationPRN
{
    public class PsychotropicDosageChangeInfoMap : ReadOnlyModelMap<PsychotropicAdministrationPRNInfo, Domain.Models.PsychotropicAdministrationPRN>
    {
        public PsychotropicDosageChangeInfoMap()
        {

            ForProperty(model => model.Id)
                .Read(domain => domain.Id.ToString());

            ForProperty(model => model.Dosage)
                .Read(FormatDosage);

            ForProperty(model => model.GivenOn)
                .Read(domain => domain.GivenOn.Value.ToString("MM/dd/yy"));
        }

        private string FormatDosage(Domain.Models.PsychotropicAdministrationPRN domain)
        {
            if (domain.Dosage.HasValue)
            {
                return string.Concat(domain.Dosage.Value, " ", domain.Administration.DosageForm.Name);
            }

            return "N/A";
        }

    }
}
