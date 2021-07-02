using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;

namespace IQI.Intuition.Web.Models.SupportCenter
{

	public class SystemTicketViewFormMap : ReadOnlyModelMap<SystemTicketViewForm, Domain.Models.SystemTicket>
	{
		public SystemTicketViewFormMap()
		{

			ForProperty(model => model.Id)
				.Read(domain => domain.Id);

            ForProperty(model => model.SystemTicketType)
                .Read(domain => domain.SystemTicketType != null ? domain.SystemTicketType.Name : string.Empty)
                .DisplayName("Type");

            ForProperty(model => model.SystemTicketStatus)
                .Read(domain => System.Enum.GetName(typeof(Domain.Enumerations.SystemTicketStatus), domain.Status).SplitPascalCase())
                .DisplayName("Status");

            ForProperty(model => model.AccountUser)
                .Read(domain => domain.AccountUser != null ? domain.AccountUser.Login : string.Empty)
                .DisplayName("User");

            ForProperty(model => model.Details)
                .Read(domain => domain.Details)
                .DisplayName("Details");


            ForProperty(model => model.CreatedOn)
                .Read(domain => domain.CreatedOn.FormatAsShortDate())
                .DisplayName("Created On");


		}
	}
}

