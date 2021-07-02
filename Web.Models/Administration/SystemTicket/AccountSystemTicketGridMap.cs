using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Administration.SystemTicket
{
    public class AccountSystemTicketGridMap : ReadOnlyModelMap<AccountSystemTicketGrid, IPagedQueryResult<Domain.Models.SystemTicket>>
    {
        public AccountSystemTicketGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}


