using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PatientCensus
{
    public class PatientCensusInfoGridMap : ReadOnlyModelMap<PatientCensusGrid, IPagedQueryResult<Domain.Models.PatientCensus>>
    {
        public PatientCensusInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
