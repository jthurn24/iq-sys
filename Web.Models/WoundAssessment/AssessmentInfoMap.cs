using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.WoundAssessment
{
    public class AssessmentInfoMap : ReadOnlyModelMap<AssessmentInfo, Domain.Models.WoundAssessment>
    {
        public AssessmentInfoMap()
        {

            AutoConfigure();

            ForProperty(model => model.Id)
                .Read(domain => domain.Id.ToString());

            ForProperty(model => model.StageName)
                .Read(domain => domain.Stage.Name);

            ForProperty(model => model.AssessmentDate)
                .Read(domain => domain.AssessmentDate.FormatAsShortDate());

            ForProperty(model => model.RoomWingName)
                .Read(x => String.Concat(x.Room.Name, " ", x.Room.Wing.Name));
        }


    }
}
