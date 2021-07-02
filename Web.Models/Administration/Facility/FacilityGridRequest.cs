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

namespace IQI.Intuition.Web.Models.Administration.Facility
{
    [ModelBinder(typeof(AjaxRequestModelBinder<FacilityGridRequest>))]
    public class FacilityGridRequest : AjaxRequestModel<FacilityGridRequest>
    {
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string SubDomain { get; set; }
        public string State { get; set; }
        public string LastSynchronizedAt { get; set; }

        public Expression<Func<Domain.Models.Facility, object>> SortBy
        {
            get
            {
                if (RequestedSortBy(model => model.Guid))
                {
	                return x => x.Guid;
                }
                if (RequestedSortBy(model => model.Name))
                {
	                return x => x.Name;
                }
                if (RequestedSortBy(model => model.SubDomain))
                {
	                return x => x.SubDomain;
                }
                if (RequestedSortBy(model => model.State))
                {
	                return x => x.State;
                }
                if (RequestedSortBy(model => model.LastSynchronizedAt))
                {
	                return x => x.LastSynchronizedAt;
                }
              
                return x => x.Id;
            }
        }
    }
}

