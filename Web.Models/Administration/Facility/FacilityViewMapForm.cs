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

namespace IQI.Intuition.Web.Models.Administration.Facility
{

	public class FacilityViewFormMap : ReadOnlyModelMap<FacilityViewForm, Domain.Models.Facility>
	{
		public FacilityViewFormMap()
		{

			ForProperty(model => model.Id)
				.Read(domain => domain.Id);

            ForProperty(model => model.Guid)
	            .Read(domain => domain.Guid.ToString())
                .DisplayName("GUID");

            ForProperty(model => model.Name)
	            .Read(domain => domain.Name)
                .DisplayName("Name");

            ForProperty(model => model.SubDomain)
	            .Read(domain => domain.SubDomain)
                .DisplayName("Sub Domain");

            ForProperty(model => model.State)
	            .Read(domain => domain.State)
                .DisplayName("State");

            ForProperty(model => model.AccountId)
	            .Read(domain => domain.Account.Id);

            ForProperty(model => model.FacilityType)
                .Read(domain => System.Enum.GetName(typeof(Domain.Enumerations.FacilityType),domain.FacilityType).SplitPascalCase()  )
                .DisplayName("Type");

            ForProperty(model => model.MaxBeds)
                .Read(domain => domain.MaxBeds.ToStringSafely())
                .DisplayName("Capacity (Beds)");


		}
	}
}

