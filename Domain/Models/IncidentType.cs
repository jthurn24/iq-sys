using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class IncidentType : AuditTrackingEntity<IncidentType>
    {
        public virtual string Name { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual string GroupName { get; set; }
        public virtual string Color { get; set; }
        public virtual string GroupColor { get; set; }

        public virtual IList<IncidentReport> IncidentReports { get; set; }
    }
}
