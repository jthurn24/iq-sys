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

namespace IQI.Intuition.Web.Models.Configuration.Wing
{
    public class WingEntryMap : ModelMap<WingEntry, Domain.Models.Wing>
    {
        private IActionContext _ActionContext { get; set; }
        private IFacilityRepository _FacilityRepository { get; set; }

        public WingEntryMap(
            IActionContext actionContext,
            IFacilityRepository facilityRepository)
        {
            _ActionContext = actionContext;
            _FacilityRepository = facilityRepository;

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

            ForProperty(model => model.FloorId)
                .Read(x => x.Floor.Id)
                .Write(SetFloor)
                .HiddenInput();

            ForProperty(model => model.Name)
                .Bind(domain => domain.Name)
                .Required();
        }

        private void SetFloor(Domain.Models.Wing model, int? id)
        {
            model.Floor = this._ActionContext.CurrentFacility.Floors.Where(x => x.Id == id.Value).FirstOrDefault();
        }


    }
}
