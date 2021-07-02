using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Infrastructure.Services.BusinessLogic.FacilityTimeLine.EventSource.Patient
{
    public class MovementState : BaseState
    {
        public Domain.Enumerations.PatientStatus Status { get; set; }
        public Domain.Models.Room Room { get; set; }
        public Domain.Models.Patient Patient { get; set; }
    }
}
