using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.Extensions.Formatting;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Extensions;

namespace IQI.Intuition.Web.Models.Administration.FacilityProduct
{

	public class FacilityProductGridItemMap : ReadOnlyModelMap<FacilityProductGridItem, Domain.Models.FacilityProduct>
	{
		public FacilityProductGridItemMap()
		{
			AutoConfigure();

			ForProperty(model => model.Id)
				.Read(domain => domain.Id);

            ForProperty(model => model.Fee)
	            .Read(domain => domain.Fee.ToString());

            ForProperty(model => model.FeeType)
                .Read(domain => System.Enum.GetName(typeof(Domain.Enumerations.ProductFeeType), domain.FeeType).SplitPascalCase());

            ForProperty(model => model.SystemProduct)
                .Read(domain => domain.SystemProduct != null ? domain.SystemProduct.Name : string.Empty);

            ForProperty(model => model.StartOn)
                .Read(domain => domain.StartOn.FormatAsShortDate());

		}
	}
}

