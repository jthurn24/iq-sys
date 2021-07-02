using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Precaution
{
    public class PatientPrecautionMap : ReadOnlyModelMap<PatientPrecautionInfo, PatientPrecaution>
    {
        public PatientPrecautionMap()
        {
            AutoConfigure();

        }
    }
}
