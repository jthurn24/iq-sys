using System;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;

namespace IQI.Intuition.Web.Models.Patient
{
    public class PatientInfoGridMap : ReadOnlyModelMap<PatientGrid, IPagedQueryResult<Domain.Models.Patient>>
    {
        public PatientInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
