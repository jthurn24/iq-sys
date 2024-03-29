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
    public class FacilityGridItem
    {
        public int? Id { get; set; }
        public System.Guid Guid { get; set; }
        public string Name { get; set; }
        public string SubDomain { get; set; }
        public string State { get; set; }
        public int PatientCount { get; set; }
    }
}

