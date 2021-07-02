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

namespace IQI.Intuition.Web.Models.SupportCenter
{
    public class SystemTicketAddFormMap : ModelMap<SystemTicketAddForm, Domain.Models.SystemTicket>
    {

        public SystemTicketAddFormMap()
        {

            ForProperty(model => model.Details)
                .Bind(domain => domain.Details)
	            .DisplayName("Details");

        }


    }
}


