using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Catheter
{
    public class CatheterInfoGridMap : ReadOnlyModelMap<CatheterGrid, IPagedQueryResult<CatheterEntry>>
    {
        public CatheterInfoGridMap()
        {
            AutoConfigure();

            ForProperty(model => model.PageItems)
                .Map(domain => domain.PageValues);
        }
    }
}
