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

namespace IQI.Intuition.Web.Models.Administration.SystemSecureFile
{

	public class SystemSecureFileViewFormMap : ReadOnlyModelMap<SystemSecureFileViewForm, Domain.Models.SystemSecureFile>
	{
		public SystemSecureFileViewFormMap()
		{
			AutoConfigure();

			ForProperty(model => model.Id)
				.Read(domain => domain.Id);

            ForProperty(model => model.Description)
	            .Read(domain => domain.Description);

		}
	}
}

