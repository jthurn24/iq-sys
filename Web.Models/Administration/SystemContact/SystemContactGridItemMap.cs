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

namespace IQI.Intuition.Web.Models.Administration.SystemContact
{

	public class SystemContactGridItemMap : ReadOnlyModelMap<SystemContactGridItem, Domain.Models.SystemContact>
	{
		public SystemContactGridItemMap()
		{
			AutoConfigure();

			ForProperty(model => model.Id)
				.Read(domain => domain.Id);

            ForProperty(model => model.FirstName)
	            .Read(domain => domain.FirstName);

            ForProperty(model => model.LastName)
	            .Read(domain => domain.LastName);

            ForProperty(model => model.Title)
	            .Read(domain => domain.Title);

            ForProperty(model => model.Cell)
	            .Read(domain => domain.Cell);

            ForProperty(model => model.Direct)
	            .Read(domain => domain.Direct);

            ForProperty(model => model.Email)
	            .Read(domain => domain.Email);

            ForProperty(model => model.Notes)
	            .Read(domain => domain.Notes);

		}
	}
}

