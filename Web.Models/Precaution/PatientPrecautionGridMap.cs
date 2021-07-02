using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Precaution
{
    public class PatientPrecautionInfoGridMap : ReadOnlyModelMap<PatientPrecautionGrid, IPagedQueryResult<Domain.Models.PatientPrecaution>>
    {
        public PatientPrecautionInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
