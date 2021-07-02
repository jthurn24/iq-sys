using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.WoundAssessment
{
    public class ReportAssessmentGridMap : ReadOnlyModelMap<ReportAssessmentGrid, IPagedQueryResult<Domain.Models.WoundAssessment>>
    {
        public ReportAssessmentGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
