using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class PatientCensus : AuditTrackingEntity<PatientCensus>
    {
        public virtual Facility Facility { get; set; }
        public virtual int Month { get; set; }
        public virtual int Year { get; set; }
        public virtual int PatientDays { get; set; }
        public virtual DateTime? LastSynchronizedAt { get; set; }
    }
}
