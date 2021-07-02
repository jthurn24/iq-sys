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

namespace IQI.Intuition.Web.Models.Administration.FacilityProduct
{
    public class FacilityProductAddForm
    {
		public int? Id { get; set; }
        public decimal Fee { get; set; }
        public Enumerations.ProductFeeType FeeType { get; set; }
        public int? SystemProductId { get; set; }
        public DateTime? StartOn { get; set; }
        public string Note { get; set; }
    }
}

