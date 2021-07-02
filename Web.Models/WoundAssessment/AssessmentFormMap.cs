using System;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.Extensions;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using System.Linq;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.Extensions;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Web.Models.WoundAssessment
{
    public class AssessmentFormMap : ModelMap<AssessmentForm, Domain.Models.WoundAssessment>
    {
        protected IWoundRepository WoundRespository { get; set; }

        public AssessmentFormMap(
           IActionContext actionContext,
            IWoundRepository woundRespository)
        {

     
            WoundRespository = woundRespository;

            ForProperty(model => model.Id)
                .Read(domain => domain != null ? domain.Id.ToString() : string.Empty)
            .ReadOnly();

            ForProperty(model => model.Stage)
                 .Bind(domain => domain.Stage)
                     .OnRead(x => x.Id)
                     .OnWrite(x => SelectStage(x))
                 .Required()
                 .DropDownList(GetStages)
                 .DisplayName("Stage");

            ForProperty(model => model.AssessmentDate)
                .Bind(domain => domain.AssessmentDate)
                .DisplayName("Assessment Date")
                .Required();

            ForProperty(model => model.ClientData)
                .Map(domain => domain)
                .ReadOnly();

            ForProperty(model => model.Floor)
                .Read(domain => domain.Room.Wing.Floor.Id)
                .DropDownList();

            ForProperty(model => model.Wing)
                .Read(domain => domain.Room.Wing.Id)
                .DropDownList();

            ForProperty(model => model.Room)
                .Bind(domain => domain.Room)
                    .OnRead(room => room.Id)
                    .OnWrite(roomId => SelectFacilityRoom(actionContext.CurrentFacility, roomId))
                .Required()
                .DropDownList();



             ForProperty(model => model.Lcm)
                    .Bind(domain => domain.Lcm)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value,2) : 0)
                    .DisplayName("L");

             ForProperty(model => model.Wcm)
                    .Bind(domain => domain.Wcm)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("W");

             ForProperty(model => model.Dcm)
                    .Bind(domain => domain.Dcm)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("D");

             ForProperty(model => model.Superficial)
                    .Bind(domain => domain.Superficial);

             ForProperty(model => model.Undermining1From)
                    .Bind(domain => domain.Undermining1From)
                    .DisplayName("O'clock from");

             ForProperty(model => model.Undermining1To)
                    .Bind(domain => domain.Undermining1To)
                    .DisplayName("O'clock to");

             ForProperty(model => model.Undermining1Depth)
                    .Bind(domain => domain.Undermining1Depth)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Depth (cm)");

             ForProperty(model => model.Undermining2From)
                    .Bind(domain => domain.Undermining2From)
                    .DisplayName("O'clock from");

             ForProperty(model => model.Undermining2To)
                    .Bind(domain => domain.Undermining2To)
                    .DisplayName("O'clock to");

             ForProperty(model => model.Undermining2Depth)
                    .Bind(domain => domain.Undermining2Depth)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Depth (cm)");


             ForProperty(model => model.Tunnel1Location)
                    .Bind(domain => domain.Tunnel1Location)
                    .DisplayName("O'clock");

             ForProperty(model => model.Tunnel1Depth)
                    .Bind(domain => domain.Tunnel1Depth)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Depth (cm)");

             ForProperty(model => model.Tunnel2Location)
                    .Bind(domain => domain.Tunnel2Location)
                    .DisplayName("O'clock");

             ForProperty(model => model.Tunnel2Depth)
                    .Bind(domain => domain.Tunnel2Depth)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Depth (cm)");


             ForProperty(model => model.Tunnel3Location)
                    .Bind(domain => domain.Tunnel3Location)
                    .DisplayName("O'clock");

             ForProperty(model => model.Tunnel3Depth)
                    .Bind(domain => domain.Tunnel3Depth)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Depth (cm)");

             ForProperty(model => model.Exudate)
                    .Bind(domain => domain.Exudate)
                    .EnumList(); ;

             ForProperty(model => model.ExudateType)
                    .Bind(domain => domain.ExudateType)
                    .EnumList(); ;

             ForProperty(model => model.Odor)
                    .Bind(domain => domain.Odor);

             ForProperty(model => model.WoundBedEpithelial)
                    .Bind(domain => domain.WoundBedEpithelial)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Epithelial %");

             ForProperty(model => model.WoundBedGranulation)
                    .Bind(domain => domain.WoundBedGranulation)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Granulation %");

             ForProperty(model => model.WoundBedSlough)
                    .Bind(domain => domain.WoundBedSlough)
                     .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Slough %");

             ForProperty(model => model.WoundBedNecrosis)
                    .Bind(domain => domain.WoundBedNecrosis)
                    .OnRead(domain => domain.HasValue ? Math.Round(domain.Value, 2) : 0)
                    .DisplayName("Necrosis/Escar %");

             ForProperty(model => model.WoundBedOther)
                    .Bind(domain => domain.WoundBedOther)
                    .DisplayName("Other");

             ForProperty(model => model.WoundEdge)
                    .Bind(domain => domain.WoundEdge)
                    .DisplayName("Wound Edges")
                    .EnumList(); ;

             ForProperty(model => model.PeriwoundTissue)
                    .Bind(domain => domain.PeriwoundTissue)
                    .DisplayName("Periwound Tissue")
                    .EnumList(); ;

             ForProperty(model => model.Pain)
                    .Bind(domain => domain.Pain);

             ForProperty(model => model.PainManagedWith)
                    .Bind(domain => domain.PainManagedWith)
                    .DisplayName("Pain managed with");

             ForProperty(model => model.Progress)
                    .Bind(domain => domain.Progress)
                    .EnumList(); ;

             ForProperty(model => model.PushScore)
                    .Bind(domain => domain.PushScore)
                    .DisplayName("Push Score");

             ForProperty(model => model.TreatmentStatus)
                    .Bind(domain => domain.TreatmentStatus)
                    .DisplayName("Treatment")
                    .EnumList();

        }

        private Room SelectFacilityRoom(Facility facility, int? roomId)
        {
            if (!roomId.HasValue)
            {
                return null;
            }

            return facility.Floors
                .SelectMany(floor => floor.Wings)
                .SelectMany(wing => wing.Rooms)
                .SingleOrDefault(room => room.Id == roomId);
        }

        public WoundStage SelectStage(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return WoundRespository.AllStages.Where(x => x.Id == id.Value).FirstOrDefault();
        }

        public IEnumerable<SelectListItem> GetStages()
        {
            return WoundRespository.AllStages
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                .Prepend(new SelectListItem() { Text = string.Empty, Value = string.Empty });
        }

    }
}
