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

namespace IQI.Intuition.Web.Models.SupportCenter
{
    [ModelBinder(typeof(AjaxRequestModelBinder<SystemTicketGridRequest>))]
    public class SystemTicketGridRequest : AjaxRequestModel<SystemTicketGridRequest>
    {
		public int? Id { get; set; }
        public string SystemTicketType { get; set; }
        public string SystemTicketStatus { get; set; }
        public string Account { get; set; }
        public string AccountUser { get; set; }
        public string Details { get; set; }
        public int Priority { get; set; }
        public string SystemUser { get; set; }
        public string CreatedOn { get; set; }
        public string ClosedOn { get; set; }
        public string Release { get; set; }

        public Expression<Func<Domain.Models.SystemTicket, object>> SortBy
        {
            get
            {
                if (RequestedSortBy(model => model.SystemTicketType))
                {
	                return x => x.SystemTicketType.Name;
                }
                if (RequestedSortBy(model => model.SystemTicketStatus))
                {
	                return x => x.Status;
                }
                if (RequestedSortBy(model => model.Account))
                {
	                return x => x.Account.Name;
                }
                if (RequestedSortBy(model => model.AccountUser))
                {
	                return x => x.AccountUser.Login;
                }
                if (RequestedSortBy(model => model.Priority))
                {
	                return x => x.Priority;
                }
                if (RequestedSortBy(model => model.SystemUser))
                {
	                return x => x.SystemUser.Login;
                }
                if (RequestedSortBy(model => model.CreatedOn))
                {
	                return x => x.CreatedOn;
                }
                if (RequestedSortBy(model => model.ClosedOn))
                {
	                return x => x.ClosedOn;
                }
                if (RequestedSortBy(model => model.Release))
                {
                    return x => x.Release;
                }
              
                return x => x.Id;
            }
        }
    }
}

