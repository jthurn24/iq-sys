using System;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Warning
{
    public class WarningInfoMap : ReadOnlyModelMap<WarningInfo, Domain.Models.Warning>
    {
        public WarningInfoMap()
        {
            AutoConfigure();

            ForProperty(model => model.TriggeredOn)
                .Read(domain => domain.TriggeredOn.FormatAsShortDate())
                .DisplayName("On");
            
            ForProperty(model => model.Target)
                .Read(domain => System.Enum.GetName(typeof(WarningTarget), domain.GetTarget()));

            ForProperty(model => model.PatientName)
                .Read(domain => domain.Patient != null ? domain.Patient.FullName : string.Empty)
                .DisplayName("Patient");

            ForProperty(model => model.Recent)
                .Read(domain => DateTime.Today.Subtract(domain.TriggeredOn).Days < 7)
                .DisplayName("Recent");

        }
    }
}
