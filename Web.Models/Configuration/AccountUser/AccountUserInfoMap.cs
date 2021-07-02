using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Configuration.AccountUser
{
    public class AccountUserInfoMap : ReadOnlyModelMap<AccountUserInfo, Domain.Models.AccountUser>
    {
        public AccountUserInfoMap()
        {
            AutoConfigure();
        }
    }
}
