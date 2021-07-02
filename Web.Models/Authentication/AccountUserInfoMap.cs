using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Authentication
{
    public class AccountUserInfoMap : ReadOnlyModelMap<AccountUserInfo, AccountUser>
    {
        public AccountUserInfoMap()
        {
            AutoConfigure();
            
            ForProperty(model => model.Name)
                .Read(domain => domain.FirstName);
        }
    }
}
