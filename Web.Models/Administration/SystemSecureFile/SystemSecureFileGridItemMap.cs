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

namespace IQI.Intuition.Web.Models.Administration.SystemSecureFile
{

	public class SystemSecureFileGridItemMap : ReadOnlyModelMap<SystemSecureFileGridItem, Domain.Models.SystemSecureFile>
	{
		public SystemSecureFileGridItemMap()
		{
			AutoConfigure();

			ForProperty(model => model.Id)
				.Read(domain => domain.Id.ToString());

            ForProperty(model => model.Description)
	            .Read(domain => domain.Description);

            ForProperty(model => model.ExpiresOn)
                .Read(domain => domain.ExpiresOn.FormatAsShortDate());

            ForProperty(model => model.CreatedOn)
                .Read(domain => domain.CreatedOn.FormatAsShortDate());

		}
	}
}

