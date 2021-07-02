using System;
using System.Linq;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Domain;

namespace IQI.Intuition.Web.Models.EmployeeInfection
{
    // This map is used to populate the ClientData model via DI, but we still need to reference a "real" domain model
    public class EmployeeInfectionFormClientDataMap : ModelMap<EmployeeInfectionFormClientData, Domain.Models.EmployeeInfection>
    {
        public EmployeeInfectionFormClientDataMap(IActionContext actionContext)
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
        }
    }
}