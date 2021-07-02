using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;

namespace IQI.Intuition.Domain.Models
{
    public class AuditEntry : Entity<AuditEntry>, IRestrictable<Account>
    {

        public const int DETAILS_MODE_SERIALIZED_CHANGES = 1;

        public virtual DateTime PerformedAt { get; set; }
        public virtual string PerformedBy { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual string DetailsText { get; set; }
        public virtual Enumerations.AuditEntryType AuditType { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual Guid? TargetPatient { get; set; }
        public virtual Guid? TargetComponent { get; set; }
        public virtual int? DetailsMode { get; set; }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Facility.Account.Id;
        }
    }
}
