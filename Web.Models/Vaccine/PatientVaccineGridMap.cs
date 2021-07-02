using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Vaccine
{
    public class PatientVaccineGridMap : ReadOnlyModelMap<PatientVaccineGrid, IPagedQueryResult<VaccineEntry>>
    {
        public PatientVaccineGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
