using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class CmsNote : AuditTrackingEntity<CmsNote>
    {
        public virtual Patient Patient { get; set;}
        public virtual string NoteText { get; set; }
        public virtual Boolean IsCurrent { get; set; }
    }
}
