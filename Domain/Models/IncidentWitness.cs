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
    public class IncidentWitness : AuditTrackingEntity<IncidentWitness>, IRestrictable<Account>, ITrackChanges
    {
        public virtual IncidentReport IncidentReport { get; set; }
        public virtual string Name { get; set; }
        public virtual string Statement { get; set; }
        public virtual string Role { get; set; }


        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.IncidentReport.Patient.Account.Id;
        }


        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<IncidentWitness>()
            .CreateFlag(Enumerations.AuditEntryType.ModifiedIncident)
            .EditFlag(Enumerations.AuditEntryType.ModifiedIncident)
            .RemoveFlag(Enumerations.AuditEntryType.ModifiedIncident)
            .PatientGuid(x => x.IncidentReport.Patient.Guid)
            .ComponentGuid(x => x.IncidentReport.Guid)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .GetDefinition();
        }
    }
}
