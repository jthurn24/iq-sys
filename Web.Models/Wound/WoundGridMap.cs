using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Wound
{
    public class WoundGridMap : ReadOnlyModelMap<WoundGrid, IPagedQueryResult<Domain.Models.WoundReport>>
    {
        public WoundGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
