using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Configuration.AccountUser
{
    public class AccountUserInfoGridMap : ReadOnlyModelMap<AccountUserGrid, IPagedQueryResult<Domain.Models.AccountUser>>
    {
        public AccountUserInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
