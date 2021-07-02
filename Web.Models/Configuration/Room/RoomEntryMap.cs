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

namespace IQI.Intuition.Web.Models.Configuration.Room
{
    public class RoomEntryMap : ModelMap<RoomEntry, Domain.Models.Room>
    {
        private IActionContext _ActionContext { get; set; }
        private IFacilityRepository _FacilityRepository { get; set; }

        public RoomEntryMap(
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

            ForProperty(model => model.WingId)
                .Read(x => x.Wing.Id)
                .Write(SetWing)
                .HiddenInput();

            ForProperty(model => model.Name)
                .Bind(domain => domain.Name)
                .Required();

            ForProperty(model => model.IsInactive)
                .Bind(domain => domain.IsInactive);
        }

        private void SetWing(Domain.Models.Room model, int? id)
        {
            model.Wing = _FacilityRepository.SearchWings(this._ActionContext.CurrentFacility.Id,id.Value).FirstOrDefault();
        }

    }
}
