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
    public class InfectionLabResultPathogen : AuditTrackingEntity<InfectionLabResultPathogen>, IRestrictable<Account>, ITrackChanges
    {
        public virtual InfectionLabResult InfectionLabResult { get; set; }
        public virtual Pathogen Pathogen { get; set; }
        public virtual PathogenQuantity PathogenQuantity { get; set; }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.InfectionLabResult.InfectionVerification.Patient.Account.Id;
        }


        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<InfectionLabResultPathogen>()
            .CreateFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .EditFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .RemoveFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .PatientGuid(x => x.InfectionLabResult.InfectionVerification.Patient.Guid)
            .ComponentGuid(x => x.InfectionLabResult.InfectionVerification.Guid)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .Description(x => x.Pathogen != null ? string.Concat(x.Pathogen.Name, " Pathogen") : "Pathogen")
            .GetDefinition();
        }
    }
}
