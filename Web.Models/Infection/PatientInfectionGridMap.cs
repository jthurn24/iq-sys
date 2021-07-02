using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Infection
{
    public class PatientInfectionGridMap : ReadOnlyModelMap<PatientInfectionGrid, IPagedQueryResult<InfectionVerification>>
    {
        public PatientInfectionGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
