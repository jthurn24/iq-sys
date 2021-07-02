using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.SupportCenter
{
    public class SystemTicketGridMap : ReadOnlyModelMap<SystemTicketGrid, IPagedQueryResult<Domain.Models.SystemTicket>>
    {
        public SystemTicketGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}


