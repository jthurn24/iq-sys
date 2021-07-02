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

namespace IQI.Intuition.Web.Models.Administration.Facility
{
    public class FacilityViewForm
    {
		public int? Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string SubDomain { get; set; }
        public string State { get; set; }
        public int? AccountId { get; set; }
        public string FacilityType { get; set; }
        public string MaxBeds { get; set; }
    }
}

