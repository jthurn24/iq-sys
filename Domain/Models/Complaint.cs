using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;

namespace IQI.Intuition.Domain.Models
{
    public class Complaint : AuditTrackingEntity<Complaint>, IRestrictable<Account>
    {


        public virtual Facility Facility { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual string DescriptionText { get; set; }
        public virtual DateTime DateReported { get; set; }
        public virtual DateTime DateOccurred { get; set; }
        public virtual Wing Wing { get; set; }
        public virtual string ReportedBy { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual ComplaintType ComplaintType { get; set; }
        public virtual bool? Reported { get; set; }
        public virtual bool? Cleared { get; set; }
        public virtual Employee Employee2 { get; set; }
        public virtual Patient Patient2 { get; set; }
        public virtual DateTime? LastSynchronizedAt { get; set; }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Facility.Account.Id;
        }
    }
}
