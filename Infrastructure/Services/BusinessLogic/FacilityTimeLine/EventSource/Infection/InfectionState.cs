using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Infection
{
    public class InfectionState : BaseState
    {
        public Domain.Models.InfectionSite InfectionSite { get; set; }
        public Domain.Models.InfectionType InfectionType { get; set; }
        public Domain.Models.InfectionClassification InfectionClassification { get; set; }
    }
}
