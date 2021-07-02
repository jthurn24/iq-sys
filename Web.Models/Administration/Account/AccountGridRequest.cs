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

namespace IQI.Intuition.Web.Models.Administration.Account
{
    [ModelBinder(typeof(AjaxRequestModelBinder<AccountGridRequest>))]
    public class AccountGridRequest : AjaxRequestModel<AccountGridRequest>
    {
		public int? Id { get; set; }
public System.Guid Guid { get; set; }
public string Name { get; set; }
public System.Collections.Generic.IEnumerable<IQI.Intuition.Domain.Models.AccountUser> Users { get; set; }
public System.Collections.Generic.IEnumerable<IQI.Intuition.Domain.Models.Facility> Facilities { get; set; }

        public Expression<Func<Domain.Models.Account, object>> SortBy
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

                return x => x.Id;
            }
        }
    }
}

