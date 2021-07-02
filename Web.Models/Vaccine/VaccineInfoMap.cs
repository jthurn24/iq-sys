using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Vaccine
{
    public class VaccineInfoMap : ReadOnlyModelMap<VaccineInfo, VaccineEntry>
    {
        public VaccineInfoMap()
        {
            AutoConfigure();

            ForProperty(model => model.VaccineType)
                .Read(domain => domain.VaccineType == null ? string.Empty : domain.VaccineType.FullVaccineName);

            ForProperty(model => model.RefusalReason)
                .Read(domain => domain.VaccineRefusalReason == null ? string.Empty : domain.VaccineRefusalReason.CodeValue);

        }
    }
}
