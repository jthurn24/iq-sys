using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class VaccineRoute : AuditTrackingEntity<VaccineRoute>
    {
        protected VaccineRoute() { }

        public virtual string Value { get; set; }
        public virtual string Description { get; set; }
    }
}
