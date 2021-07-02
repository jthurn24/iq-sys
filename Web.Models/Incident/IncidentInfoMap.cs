using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentInfoMap : ReadOnlyModelMap<IncidentInfo, IncidentReport>
    {
        public IncidentInfoMap()
        {
            AutoConfigure();

            ForProperty(model => model.IncidentTypes)
                .Read(domain => domain.IncidentTypes.Select(x => x.Name).ToList().ToDelimitedString(","));

            ForProperty(model => model.InjuryLevel)
                .Read(domain => System.Enum.GetName(typeof(Domain.Enumerations.InjuryLevel),domain.InjuryLevel).SplitPascalCase());
        }
    }
}
