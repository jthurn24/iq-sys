using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;
using IQI.Intuition.Domain.Services.ChangeTracking;

namespace IQI.Intuition.Domain.Models
{
    public class PatientPrecaution : Entity<PatientPrecaution> ,IRestrictable<Account>, ITrackChanges
    {
        public virtual Patient Patient { get; set; }
        public virtual PrecautionType PrecautionType { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual string AdditionalDescription { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual bool? Deleted { get; set; }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Patient.Account.Id;
        }

        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<PatientPrecaution>()
            .CreateFlag(Enumerations.AuditEntryType.AddedInfection)
            .EditFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .RemoveFlag(Enumerations.AuditEntryType.RemovedInfection)
            .RemoveExpression(x => x.Deleted == true)
            .PatientGuid(x => x.Patient.Guid)
            .ComponentGuid(x => x.Guid)
            .Ignore(x => x.Deleted)
            .Ignore(x => x.Patient)
            .Ignore(x => x.PrecautionType)
            .Description(x => x.PrecautionType != null ? string.Concat(x.PrecautionType.Name, " Precuation") : "Precuation")
            .GetDefinition();
        }
    }
}
