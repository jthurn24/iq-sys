using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class CmsMatrixEntry : AuditTrackingEntity<CmsMatrixEntry>
    {
        public virtual Patient Patient { get; set; }
        public virtual bool? IsCurrent { get; set; }
        public virtual string SelectedOptions { get; set; }
        public virtual CmsMatrixCategory Category { get; set; }
    }
}
