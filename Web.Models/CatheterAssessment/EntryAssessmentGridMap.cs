using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.CatheterAssessment
{
    public class EntryAssessmentGridMap : ReadOnlyModelMap<EntryAssessmentGrid, IPagedQueryResult<Domain.Models.CatheterAssessment>>
    {
        public EntryAssessmentGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
