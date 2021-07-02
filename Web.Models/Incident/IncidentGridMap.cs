using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentInfoGridMap : ReadOnlyModelMap<IncidentGrid, IPagedQueryResult<IncidentReport>>
    {
        public IncidentInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
