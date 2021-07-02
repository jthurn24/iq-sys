using System;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.PatientCensus
{
    public class PatientCensusFormMap : ModelMap<PatientCensusForm, Domain.Models.PatientCensus>
    {
        public PatientCensusFormMap()
        {
            AutoFormatDisplayNames();

            ForProperty(model => model.Id)
                .Read(domain => domain.Id)
                .HiddenInput();

            ForProperty(model => model.Month)
                .Read(domain => domain.Month)
                .HiddenInput();

            ForProperty(model => model.Year)
                .Read(domain => domain.Year)
                .HiddenInput();

            ForProperty(model => model.PatientDays)
                .Bind(domain => domain.PatientDays)
                .Required()
                .DisplayName("Patient Days");
            
            ForProperty(model => model.DaysInMonth)
                .Read(domain => new DateTime(domain.Year,domain.Month,1).AddMonths(1).AddDays(-1).Day );

        }
    }
}
