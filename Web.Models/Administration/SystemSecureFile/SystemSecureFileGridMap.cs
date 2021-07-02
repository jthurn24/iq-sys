using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Administration.SystemSecureFile
{
    public class SystemSecureFileGridMap : ReadOnlyModelMap<SystemSecureFileGrid, IPagedQueryResult<Domain.Models.SystemSecureFile>>
    {
        public SystemSecureFileGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}


