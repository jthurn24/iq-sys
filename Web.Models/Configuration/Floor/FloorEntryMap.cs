using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Web.Models.Extensions;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Web.Models.Configuration.Floor
{
    public class FloorEntryMap : ModelMap<FloorEntry, Domain.Models.Floor>
    {
        private IActionContext _ActionContext { get; set; }

        public FloorEntryMap(
            IActionContext actionContext)
        {
            _ActionContext = actionContext;

            AutoFormatDisplayNames();

            ForProperty(model => model.Id)
                .Bind(domain => domain.Id)
                .Exclude(On.Create)
                .HiddenInput();

            ForProperty(model => model.Guid)
                .Bind(domain => domain.Guid)
                .Default(GuidHelper.NewGuid)
                .Required()
                .HiddenInput();

            ForProperty(model => model.Name)
                .Bind(domain => domain.Name)
                .Required();
        }

    
    }
}
