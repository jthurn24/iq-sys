using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PsychotropicAdministrationPRN
{
    public class AdministrationPRNGridMap : ReadOnlyModelMap<AdministrationPRNGrid, IPagedQueryResult<Domain.Models.PsychotropicAdministrationPRN>>
    {
        public AdministrationPRNGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
