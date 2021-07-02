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

namespace IQI.Intuition.Web.Models.Administration.SystemTicket
{
    public class SystemTicketViewForm
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
    }
}

