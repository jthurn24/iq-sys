using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PsychotropicAdministrationGridMap : ReadOnlyModelMap<PsychotropicAdministrationGrid, IPagedQueryResult<Domain.Models.PsychotropicAdministration>>
    {
        public PsychotropicAdministrationGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
