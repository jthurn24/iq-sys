using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PatientPsychotropicAdministrationGridMap : ReadOnlyModelMap<PatientPsychotropicAdministrationGrid, IPagedQueryResult<Domain.Models.PsychotropicAdministration>>
    {
        public PatientPsychotropicAdministrationGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
