using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionLabResultMap : ReadOnlyModelMap<InfectionLabResult, Domain.Models.InfectionLabResult>
    {
        public InfectionLabResultMap()
        {
            ForProperty(model => model.InfectionLabResultId)
                .Read(domain => domain.Id);

            ForProperty(model => model.LabCompletedOn)
                .Read(domain => domain.CompletedOn.HasValue ? domain.CompletedOn.Value.ToString("MM/dd/yyyy") : string.Empty); 

            ForProperty(model => model.LabTestTypeId)
                .Read(domain => domain.LabTestType.Id);

            ForProperty(model => model.LabTestTypeName)
                .Read(domain => domain.LabTestType.Name);

            ForProperty(model => model.Removed)
                .Read(domain => false);

            ForProperty(model => model.LabResultName)
                .Read(domain => domain.LabResult.Name);

            ForProperty(model => model.LabResultId)
                .Read(domain => domain.LabResult.Id);

            ForProperty(model => model.Notes)
                .Read(domain => domain.Notes);

        }


    }
}
