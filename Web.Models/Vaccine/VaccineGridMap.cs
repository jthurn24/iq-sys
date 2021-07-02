using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Vaccine
{
    public class VaccineInfoGridMap : ReadOnlyModelMap<VaccineGrid, IPagedQueryResult<VaccineEntry>>
    {
        public VaccineInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
