using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PatientCensus
{
    public class PatientCensusInfoMap : ReadOnlyModelMap<PatientCensusInfo, Domain.Models.PatientCensus>
    {
        public PatientCensusInfoMap()
        {
            AutoConfigure();
        }
    }
}
