using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PsychotropicDosageChange
{
    public class AdministrationPsychotropicDosageGridMap : ReadOnlyModelMap<AdministrationPsychotropicDosageGrid, IPagedQueryResult<Domain.Models.PsychotropicDosageChange>>
    {
        public AdministrationPsychotropicDosageGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
