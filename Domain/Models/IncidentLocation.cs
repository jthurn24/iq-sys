using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class IncidentLocation : AuditTrackingEntity<IncidentLocation>
    {
        public virtual string Name { get; set; }
        public virtual string Color { get; set; }
    }
}
