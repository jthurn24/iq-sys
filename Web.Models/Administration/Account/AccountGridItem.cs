using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;
using System.Collections.Generic;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Administration.Account
{
    public class AccountGridItem
    {
        public int? Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }

    }
}

