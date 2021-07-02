using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Wound
{
    public class WoundState : BaseState
    {
        public Domain.Models.Patient Patient { get; set; }
        public Domain.Models.WoundStage WoundStage { get; set; }
        public Domain.Models.WoundType WoundType { get; set; }
        public Domain.Models.WoundReport Report { get; set; }
    }
}
