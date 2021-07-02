using System;
using System.Collections;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Web.Models.WoundAssessment
{
    public class AssessmentFormClientDataMap : ModelMap<AssessmentFormClientData, Domain.Models.WoundAssessment>
    {
        public AssessmentFormClientDataMap(
            IActionContext actionContext)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");

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

        private IActionContext ActionContext { get; set; }
    }
}
