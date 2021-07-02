using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Administration.Account
{
    public class AccountGridMap : ReadOnlyModelMap<AccountGrid, IPagedQueryResult<Domain.Models.Account>>
    {
        public AccountGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}


