using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;
using IQI.Intuition.Domain.Services.ChangeTracking;

namespace IQI.Intuition.Domain.Models
{
    public class VaccineEntry : AuditTrackingEntity<VaccineEntry>, IRestrictable<Account>
    {
        protected VaccineEntry() { }

        public VaccineEntry(Patient patient)
        {
            Patient = patient.ThrowIfNullArgument("patient");
            Guid = GuidHelper.NewGuid();
        }

        public virtual Guid Guid { get; protected set; }
        public virtual Patient Patient { get; set; }
        public virtual Room Room { get; set; }
        public virtual VaccineType VaccineType { get; set; }
        public virtual Employee AdministeringProvider { get; set; }
        public virtual DateTime? AdministeredOn { get; set; }
        public virtual VaccineRefusalReason VaccineRefusalReason { get; set; }
        public virtual bool Deleted { get; set; }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Patient.Account.Id;
        }

    }
}
