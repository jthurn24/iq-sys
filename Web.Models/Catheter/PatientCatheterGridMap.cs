using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Catheter
{
    public class PatientCatheterGridMap : ReadOnlyModelMap<PatientCatheterGrid, IPagedQueryResult<CatheterEntry>>
    {
        public PatientCatheterGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
