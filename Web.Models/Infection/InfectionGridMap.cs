using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionInfoGridMap : ReadOnlyModelMap<InfectionGrid, IPagedQueryResult<InfectionVerification>>
    {
        public InfectionInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
