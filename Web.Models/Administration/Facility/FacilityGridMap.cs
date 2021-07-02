using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Administration.Facility
{
    public class FacilityGridMap : ReadOnlyModelMap<FacilityGrid, IPagedQueryResult<Domain.Models.Facility>>
    {
        public FacilityGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}


