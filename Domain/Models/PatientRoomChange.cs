using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;

namespace IQI.Intuition.Domain.Models
{
    public class PatientRoomChange : AuditTrackingEntity<Complaint>
    {
        public virtual Patient Patient { get; set; }
        public virtual Room Room { get; set; }
        public virtual DateTime RoomChangedAt { get; set; }
    }
}
