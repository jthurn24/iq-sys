using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.ObjectModel.Metadata;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;
using IQI.Intuition.Domain.Services.ChangeTracking;



namespace IQI.Intuition.Domain.Models
{
    public class InfectionTreatment : AuditTrackingEntity<InfectionTreatment>, IRestrictable<Account>, ITrackChanges
    {
        public virtual string TreatmentName { get; set; }
        public virtual InfectionVerification InfectionVerification { get; set; }
        public virtual string Dosage { get; set; }
        public virtual string Duration { get; set; }
        public virtual string Frequency { get; set; }
        public virtual string Units { get; set; }
        public virtual string DeliveryMethod { get; set; }
        public virtual string SpecialInstructions { get; set; }
        public virtual TreatmentType TreatmentType { get; set; }
        public virtual DateTime? AdministeredOn { get; set; }
        public virtual string MDName { get; set; }
        public virtual DateTime? DiscontinuedOn { get; set; }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.InfectionVerification.Patient.Account.Id;
        }


        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<InfectionTreatment>()
            .CreateFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .EditFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .RemoveFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .PatientGuid(x => x.InfectionVerification.Patient.Guid)
            .ComponentGuid(x => x.InfectionVerification.Guid)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .Description(x => string.Concat("Treatment - ", x.TreatmentName))
            .GetDefinition();
        }
    }
}
