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

namespace IQI.Intuition.Web.Models.SupportCenter
{
    public class SystemTicketGridItem
    {
		public int? Id { get; set; }
        public string SystemTicketType { get; set; }
        public string SystemTicketStatus { get; set; }
        public string AccountUser { get; set; }
        public string Details { get; set; }
    }
}

