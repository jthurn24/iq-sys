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
using SnyderIS.sCore.Web.Mvc.JQGridHelpers;
using System.Linq.Expressions;

namespace IQI.Intuition.Web.Models.Administration.FacilityProduct
{
    [ModelBinder(typeof(AjaxRequestModelBinder<FacilityProductGridRequest>))]
    public class FacilityProductGridRequest : AjaxRequestModel<FacilityProductGridRequest>
    {
        public int? Id { get; set; }
        public string Fee { get; set; }
        public string FeeType { get; set; }
        public string SystemProduct { get; set; }
        public string StartOn { get; set; }

        public Expression<Func<Domain.Models.FacilityProduct, object>> SortBy
        {
            get
            {

                if (RequestedSortBy(model => model.Fee))
                {
	                return x => x.Fee;
                }
                if (RequestedSortBy(model => model.FeeType))
                {
	                return x => x.FeeType;
                }
                if (RequestedSortBy(model => model.SystemProduct))
                {
	                return x => x.SystemProduct;
                }
                if (RequestedSortBy(model => model.StartOn))
                {
	                return x => x.StartOn;
                }
              
                return x => x.Id;
            }
        }
    }
}

