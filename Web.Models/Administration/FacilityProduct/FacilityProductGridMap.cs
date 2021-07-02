using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Administration.FacilityProduct
{
    public class FacilityProductGridMap : ReadOnlyModelMap<FacilityProductGrid, IPagedQueryResult<Domain.Models.FacilityProduct>>
    {
        public FacilityProductGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}


