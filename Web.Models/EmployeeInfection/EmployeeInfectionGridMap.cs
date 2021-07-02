using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.EmployeeInfection
{
    public class EmployeeInfectionInfoGridMap : ReadOnlyModelMap<EmployeeInfectionGrid, IPagedQueryResult<Domain.Models.EmployeeInfection>>
    {
        public EmployeeInfectionInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
