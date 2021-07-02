using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class VaccineRefusalReason : AuditTrackingEntity<VaccineRefusalReason>
    {
        protected VaccineRefusalReason() { }

        public virtual string CodeValue { get; set; }
        public virtual string Description { get; set; }
        public virtual bool? Display { get; set; }
    }
}
