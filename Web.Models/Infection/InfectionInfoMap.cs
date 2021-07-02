using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionInfoMap : ReadOnlyModelMap<InfectionInfo, InfectionVerification>
    {
        public InfectionInfoMap()
        {
            AutoConfigure(overrides => 
            {


                overrides.ForProperty(model => model.ResolvedOn)
                    .Read(domain => domain.ResolvedOn.HasValue
                        ? domain.ResolvedOn.Value.ToShortDateString()
                        : "Not Resolved");


                overrides.ForProperty(model => model.ReasonForEntry)
                    .Read(domain => "<div style='font-size:9px;white-space:normal;color:#444;'>{0}</div>".FormatWith(domain.Criteria.Select(x => x.Name).ToDelimitedString("<br><br>")));
            });

        }


    }
}
