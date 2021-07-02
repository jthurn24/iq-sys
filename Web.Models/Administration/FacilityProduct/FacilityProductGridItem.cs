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

namespace IQI.Intuition.Web.Models.Administration.FacilityProduct
{
    public class FacilityProductGridItem
    {
        public int? Id { get; set; }
        public string Fee { get; set; }
        public string FeeType { get; set; }
        public string SystemProduct { get; set; }
        public string StartOn { get; set; }
    }
}

