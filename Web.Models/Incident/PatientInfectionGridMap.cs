using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Incident
{
    public class PatientIncidentGridMap : ReadOnlyModelMap<PatientIncidentGrid, IPagedQueryResult<IncidentReport>>
    {
        public PatientIncidentGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
