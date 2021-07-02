using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
namespace IQI.Intuition.Domain.Models
{
    public class InfectionSymptom : AuditTrackingEntity<InfectionSymptom>
    {
        public virtual string Name { get; set; }

        public virtual IEnumerable<EmployeeInfection> EmployeeInfections { get; protected set; } 
    }
}
