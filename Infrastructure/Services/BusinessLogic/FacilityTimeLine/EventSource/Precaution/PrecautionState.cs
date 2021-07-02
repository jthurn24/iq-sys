using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Precaution
{
    public class PrecautionState : BaseState
    {
        public Domain.Models.PrecautionType PrecautionType { get; set; }
    }
}
