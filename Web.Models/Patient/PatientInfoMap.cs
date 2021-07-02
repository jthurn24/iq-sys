using System;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;

namespace IQI.Intuition.Web.Models.Patient
{
    public class PatientInfoMap : ReadOnlyModelMap<PatientInfo, Domain.Models.Patient>
    {
        public PatientInfoMap()
        {
            AutoConfigure(overrides => 
                overrides.ForProperty(model => model.Status)
                    .Read(domain => (PatientInfoStatus)(int)domain.CurrentStatus));

            ForProperty(x => x.FirstName)
                .Read(x => x.GetFirstName());

            ForProperty(x => x.LastName)
                .Read(x => x.GetLastName());
        }
    }
}
