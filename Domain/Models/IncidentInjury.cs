using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class IncidentInjury : AuditTrackingEntity<IncidentInjury>
    {
        public virtual string Name { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual string Color { get; set; }

        public virtual IList<IncidentReport> IncidentReports { get; set; }
    }
}
