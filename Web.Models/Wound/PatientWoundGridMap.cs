using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Wound
{
    public class PatientWoundGridMap : ReadOnlyModelMap<PatientWoundGrid, IPagedQueryResult<Domain.Models.WoundReport>>
    {
        public PatientWoundGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
