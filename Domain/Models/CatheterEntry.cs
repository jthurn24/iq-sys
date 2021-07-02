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
    public class CatheterEntry : AuditTrackingEntity<CatheterEntry>, IRestrictable<Account>, ITrackChanges
    {

        public virtual Patient Patient { get; set; }
        public virtual string Diagnosis { get; set; }
        public virtual DateTime? StartedOn { get; set; }
        public virtual Room Room { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual DateTime? DiscontinuedOn { get; set; }
        public virtual DateTime? LastSynchronizedAt { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual int? Type { get; set; }
        public virtual int? Material { get; set; }
        public virtual int? Reason { get; set; }
        public virtual bool? PatientEducated { get; set; }
        public virtual bool? FamilyEducated { get; set; }

        public virtual IList<CatheterAssessment> Assessments { get; set; }

        protected CatheterEntry()
        {
        }

        public CatheterEntry(Patient patient)
        {
            Patient = patient.ThrowIfNullArgument("patient");
            Guid = GuidHelper.NewGuid();
            Assessments = new List<CatheterAssessment>();
        }


        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Room.Wing.Floor.Facility.Account.Id;
        }



        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<CatheterEntry>()
            .CreateFlag(Enumerations.AuditEntryType.AddedCatheter)
            .EditFlag(Enumerations.AuditEntryType.ModifiedCatheter)
            .RemoveFlag(Enumerations.AuditEntryType.RemovedCatheter)
            .RemoveExpression(x => x.Deleted == true)
            .PatientGuid(x => x.Patient.Guid)
            .ComponentGuid(x => x.Guid)
            .Ignore(x => x.Deleted)
            .Ignore(x => x.LastSynchronizedAt)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .GetDefinition();
        }

    }
}
