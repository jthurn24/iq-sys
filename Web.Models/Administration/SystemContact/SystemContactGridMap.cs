using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Administration.SystemContact
{
    public class SystemContactGridMap : ReadOnlyModelMap<SystemContactGrid, IPagedQueryResult<Domain.Models.SystemContact>>
    {
        public SystemContactGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}


