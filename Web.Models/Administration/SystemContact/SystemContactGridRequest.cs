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

namespace IQI.Intuition.Web.Models.Administration.SystemContact
{
    [ModelBinder(typeof(AjaxRequestModelBinder<SystemContactGridRequest>))]
    public class SystemContactGridRequest : AjaxRequestModel<SystemContactGridRequest>
    {
		public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Cell { get; set; }
        public string Direct { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }

        public Expression<Func<Domain.Models.SystemContact, object>> SortBy
        {
            get
            {

                if (RequestedSortBy(model => model.FirstName))
                {
	                return x => x.FirstName;
                }
                if (RequestedSortBy(model => model.LastName))
                {
	                return x => x.LastName;
                }
                if (RequestedSortBy(model => model.Title))
                {
	                return x => x.Title;
                }
                if (RequestedSortBy(model => model.Cell))
                {
	                return x => x.Cell;
                }
                if (RequestedSortBy(model => model.Direct))
                {
	                return x => x.Direct;
                }
                if (RequestedSortBy(model => model.Email))
                {
	                return x => x.Email;
                }
                if (RequestedSortBy(model => model.Notes))
                {
	                return x => x.Notes;
                }

              
                return x => x.Id;
            }
        }
    }
}

