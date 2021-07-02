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
    public class InfectionLabResult : AuditTrackingEntity<InfectionLabResult>, IRestrictable<Account>, ITrackChanges
    {
        public virtual DateTime? CompletedOn { get; set; }
        public virtual LabTestType LabTestType { get; set; }
        public virtual LabResult LabResult { get; set; }
        public virtual string Notes { get; set; }
        public virtual InfectionVerification InfectionVerification { get; set; }
        public virtual IList<InfectionLabResultPathogen> ResultPathogens { get; set; }


        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.InfectionVerification.Patient.Account.Id;
        }


        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<InfectionLabResult>()
            .CreateFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .EditFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .RemoveFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .PatientGuid(x => x.InfectionVerification.Patient.Guid)
            .ComponentGuid(x => x.InfectionVerification.Guid)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.ResultPathogens)
            .Description(x => string.Concat("Lab Test - ", x.LabTestType.Name))
            .GetDefinition();
        }


    }
}
