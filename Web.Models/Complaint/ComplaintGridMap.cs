using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Complaint
{
    public class ComplaintInfoGridMap : ReadOnlyModelMap<ComplaintGrid, IPagedQueryResult<Domain.Models.Complaint>>
    {
        public ComplaintInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
