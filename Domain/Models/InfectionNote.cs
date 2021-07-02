using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.ObjectModel.Metadata;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionNote : AuditTrackingEntity<InfectionNote>
    {
        public virtual InfectionVerification InfectionVerification { get; set; }
        public virtual string Note { get; set; }
    }
}
