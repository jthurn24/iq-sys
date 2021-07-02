using System;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper.Mapping;
using IQI.Intuition.Domain.Models;
namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentWitnessMap : ReadOnlyModelMap<IncidentWitness, Domain.Models.IncidentWitness>
    {
        public IncidentWitnessMap()
        {
            ForProperty(model => model.IncidentWitnessId)
                .Read(domain => domain.Id);

            ForProperty(model => model.Name)
                .Read(domain => domain.Name);

            ForProperty(model => model.Statement)
                .Read(domain => domain.Statement);

            ForProperty(model => model.Role)
                .Read(domain => domain.Role); 

        }
    }
}
