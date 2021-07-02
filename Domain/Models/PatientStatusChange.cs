using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class PatientStatusChange : AuditTrackingEntity<PatientStatusChange>
    {
        public virtual Patient Patient { get; set; }
        public virtual Enumerations.PatientStatus Status { get; set; }
        public virtual DateTime StatusChangedAt { get; set; }
    }
}
