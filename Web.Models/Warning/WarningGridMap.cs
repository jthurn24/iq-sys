using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Warning
{
    public class WarningInfoGridMap : ReadOnlyModelMap<WarningGrid, IPagedQueryResult<Domain.Models.Warning>>
    {
        public WarningInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
