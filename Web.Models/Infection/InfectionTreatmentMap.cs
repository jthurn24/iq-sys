using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionTreatmentMap : ReadOnlyModelMap<InfectionTreatment, Domain.Models.InfectionTreatment>
    {
        public InfectionTreatmentMap()
        {

            ForProperty(model => model.Id)
                .Read(domain => domain.Id);

            ForProperty(model => model.Removed)
                .Read(domain => false);

            ForProperty(model => model.Dosage)
                .Read(domain => domain.Dosage);

            ForProperty(model => model.Frequency)
                .Read(domain => domain.Frequency);

            ForProperty(model => model.Units)
                .Read(domain => domain.Units);

            ForProperty(model => model.DeliveryMethod)
                .Read(domain => domain.DeliveryMethod);

            ForProperty(model => model.SpecialInstructions)
                .Read(domain => domain.SpecialInstructions);

            ForProperty(model => model.TreatmentName)
                .Read(domain => domain.TreatmentName);

            ForProperty(model => model.MDName)
                .Read(domain => domain.MDName);

            ForProperty(model => model.Duration)
                .Read(domain => domain.Duration);

            ForProperty(model => model.AdministeredOn)
                .Read(domain => domain.AdministeredOn.HasValue ? domain.AdministeredOn.Value.ToString("MM/dd/yy") : string.Empty);

            ForProperty(model => model.DiscontinuedOn)
                .Read(domain => domain.DiscontinuedOn.HasValue ? domain.DiscontinuedOn.Value.ToString("MM/dd/yy") : string.Empty);

        }
    }
}
