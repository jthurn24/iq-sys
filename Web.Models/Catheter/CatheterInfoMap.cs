using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Catheter
{
    public class CatheterInfoMap : ReadOnlyModelMap<CatheterInfo, CatheterEntry>
    {
        public CatheterInfoMap()
        {
            AutoConfigure();

            ForProperty(model => model.Reason)
            .Read(domain => domain.Reason.HasValue ?
                System.Enum.GetName(typeof(Domain.Enumerations.CatheterReason), domain.Reason).SplitPascalCase()
                : string.Empty);

            ForProperty(model => model.Material)
            .Read(domain => domain.Material.HasValue ?
                System.Enum.GetName(typeof(Domain.Enumerations.CatheterMaterial), domain.Material).SplitPascalCase()
                : string.Empty);

            ForProperty(model => model.Type)
            .Read(domain => domain.Type.HasValue ?
                System.Enum.GetName(typeof(Domain.Enumerations.CatheterType), domain.Type).SplitPascalCase()
                : string.Empty);

        }
    }
}
