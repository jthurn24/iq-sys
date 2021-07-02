using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.CatheterAssessment
{
    public class AssessmentInfoMap : ReadOnlyModelMap<AssessmentInfo, Domain.Models.CatheterAssessment>
    {
        public AssessmentInfoMap()
        {

            AutoConfigure();

            ForProperty(model => model.Id)
                .Read(domain => domain.Id.ToString());

            ForProperty(model => model.AssessmentDate)
                .Read(domain => domain.AssessmentDate.FormatAsShortDate());

            ForProperty(model => model.NextAssessmentDate)
                .Read(domain => domain.NextAssessmentDate.FormatAsShortDate());

            ForProperty(model => model.RoomWingName)
                .Read(x => String.Concat(x.Room.Name, " ", x.Room.Wing.Name));
        }
    }
}
