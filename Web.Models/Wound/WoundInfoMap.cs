using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Web.Models.Wound
{
    public class WoundInfoMap : ReadOnlyModelMap<WoundInfo, Domain.Models.WoundReport>
    {
        public WoundInfoMap()
        {

            AutoConfigure();

            ForProperty(model => model.PatientGuid)
                .Read(domain => domain.Patient.Guid.ToString());

            ForProperty(model => model.PatientId)
                 .Read(domain => domain.Patient.Id);

            ForProperty(model => model.FirstNoted)
                .Read(domain => domain.FirstNotedOn.FormatAsShortDate());

            ForProperty(model => model.ResolvedOn)
                .Read(domain => domain.ResolvedOn.FormatAsShortDate());

            ForProperty(model => model.StageName)
                .Read(domain => domain.CurrentStage != null ? domain.CurrentStage.Name : string.Empty);

            ForProperty(model => model.TypeName)
                .Read(domain => domain.WoundType.Name);

            ForProperty(model => model.AdditionalSiteDetails)
                .Read(domain => domain.AdditionalSiteDetails);

            ForProperty(model => model.LocationX)
                .Read(domain => domain.LocationX.HasValue ? domain.LocationX.Value : 0);

            ForProperty(model => model.LocationY)
                .Read(domain => domain.LocationY.HasValue ? domain.LocationY.Value : 0);

            ForProperty(model => model.Id)
                .Read(domain => domain.Id.ToString());

            ForProperty(model => model.RoomWingName)
                .Read(x => x.Room != null ? String.Concat(x.Room.Name," ",x.Room.Wing.Name) : string.Empty);

            ForProperty(model => model.PushChart)
                .Read(PushChart);

        }


        protected SeriesLineChart PushChart(Domain.Models.WoundReport report)
        {
            var chart = new SeriesLineChart();


            foreach (var assessment in report.Assessments.OrderBy(x => x.AssessmentDate))
            {
                chart.AddItem(new SeriesLineChart.Item()
                {
                     Category = assessment.AssessmentDate.Value.ToString("MM/dd"),
                     Series = "Score",
                     Value = assessment.PushScore.HasValue ? assessment.PushScore.Value : 0
                });
            }

            return chart;
        }

    }
}
