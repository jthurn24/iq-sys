using System;
using System.Linq;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Web.Models.Patient
{
    // This map is used to populate the ClientData model via DI, but we still need to reference a "real" domain model
    public class PatientFormClientDataMap : ModelMap<PatientFormClientData, Domain.Models.Patient>
    {
        public PatientFormClientDataMap(IActionContext actionContext)
        {
            ForProperty(model => model.Floors)
                .Assign(() => actionContext.CurrentFacility.Floors
                    .Select(floor => new
                    {
                        Text = floor.Name,
                        Value = floor.Id
                    })
                    .ToArray());

            ForProperty(model => model.Wings)
                .Assign(() => actionContext.CurrentFacility.Floors
                    .SelectMany(floor => floor.Wings)
                    .Select(wing => new
                    {
                        Text = wing.Name,
                        Value = wing.Id,
                        Floor = wing.Floor.Id
                    })
                    .ToArray());

            ForProperty(model => model.Rooms)
                .Assign(() => actionContext.CurrentFacility.Floors
                    .SelectMany(floor => floor.Wings)
                    .SelectMany(wing => wing.Rooms)
                    .OrderBy(x => x.IsInactive).ThenBy(x => x.Name)
                    .Select(room => new
                    {
                        Text = room.IsInactive == true ? string.Concat("(inactive) ", room.Name) : room.Name,
                        Value = room.Id,
                        Wing = room.Wing.Id
                    })
                    .ToArray());
        }
    }
}