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
using IQI.Intuition.Domain;

namespace IQI.Intuition.Web.Models.Administration.SystemTicket
{
    public class SystemTicketEditForm
    {
		public int? Id { get; set; }
        public int? SystemTicketType { get; set; }
        public Enumerations.SystemTicketStatus SystemTicketStatus { get; set; }
        public string Details { get; set; }
        public int Priority { get; set; }
        public string Release { get; set; }
        public int? SystemUser { get; set; }
    }
}

