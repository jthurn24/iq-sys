using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class Room : AuditTrackingEntity<Room>
    {
        public virtual Guid Guid { get; set; }

        public virtual Wing Wing { get; set; }

        public virtual string Name { get; set; }

        public virtual bool? IsInactive { get; set; }

        public virtual IEnumerable<Patient> Patients { get; protected set; }
    }
}
