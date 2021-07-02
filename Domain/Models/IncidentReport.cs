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
    public class IncidentReport : AuditTrackingEntity<IncidentReport>, IRestrictable<Account>, ITrackChanges
    {
        public virtual Patient Patient { get; set; }
        public virtual Guid Guid { get; protected set; }
        public virtual DateTime? DiscoveredOn { get; set; }
        public virtual DateTime? OccurredOn { get; set; }
        public virtual string ResidentStatement { get; set; }
        public virtual string InjuryAndTreatmentDescription { get; set; }
        public virtual Enumerations.InjuryLevel InjuryLevel { get; set; }
        public virtual string Temperature { get; set; }
        public virtual string Pulse { get; set; }
        public virtual string Respiratory { get; set; }
        public virtual string BloodPressureStanding { get; set; }
        public virtual string BloodPressureSitting { get; set; }
        public virtual bool? NeuroCheckCompleted { get; set; }
        public virtual string BloodGlucos { get; set; }
        public virtual IncidentLocation IncidentLocation { get; set; }
        public virtual string LocationDetails { get; set; }
        public virtual bool? AssessmentCompleted { get; set; }
        public virtual Room Room { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual DateTime? LastSynchronizedAt { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Employee Employee2 { get; set; }

        private IList<IncidentType> _IncidentTypes;
        public virtual IList<IncidentType> IncidentTypes
        {
            get
            {
                return _IncidentTypes;
            }
        }

        private IList<IncidentInjury> _IncidentInjuries;
        public virtual IList<IncidentInjury> IncidentInjuries
        {
            get
            {
                return _IncidentInjuries;
            }
        }

        public virtual IList<IncidentWitness> IncidentWitnesses { get; set; }

        protected IncidentReport()
        {
        }

        public IncidentReport(Patient patient)
        {
            Patient = patient.ThrowIfNullArgument("patient");
            Guid = GuidHelper.NewGuid();
            _IncidentTypes = new List<IncidentType>();
            _IncidentInjuries = new List<IncidentInjury>();
        }

        public virtual void AssignTypes(params IncidentType[] values)
        {
            UpdateList(_IncidentTypes, values);
        }

        public virtual void AssignInjuries(params IncidentInjury[] values)
        {
            UpdateList(_IncidentInjuries, values);
        }

        private void UpdateList<T>(IList<T> list, T[] values)
        {
            values.ThrowIfNullArgument("values");
            list.Remove(item => !values.Contains(item));
            list.AddRange(values.Except(list));
        }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Patient.Account.Id;
        }


        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<IncidentReport>()
            .CreateFlag(Enumerations.AuditEntryType.AddedIncident)
            .EditFlag(Enumerations.AuditEntryType.ModifiedIncident)
            .RemoveFlag(Enumerations.AuditEntryType.RemovedIncident)
            .RemoveExpression(x => x.Deleted == true)
            .PatientGuid(x => x.Patient.Guid)
            .ComponentGuid(x => x.Guid)
            .Ignore(x => x.Deleted)
            .Ignore(x => x.LastSynchronizedAt)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .OverrideCollection(x => x.IncidentInjuries, x => x != null && x.Count() > 0 ? x.Select(xx => ((IncidentInjury)xx).Name).ToDelimitedString(',') : string.Empty)
            .OverrideCollection(x => x.IncidentTypes, x => x != null && x.Count() > 0 ? x.Select(xx => ((IncidentType)xx).Name).ToDelimitedString(',') : string.Empty)
            .Ignore(x => x.IncidentWitnesses)
            .GetDefinition();
        }

    }
}
