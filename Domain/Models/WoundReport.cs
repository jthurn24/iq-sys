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
    public class WoundReport : AuditTrackingEntity<WoundReport>, IRestrictable<Account>, ITrackChanges
    {
        public virtual Patient Patient { get; set; }
        public virtual int? LocationX { get; set; }
        public virtual int? LocationY { get; set; }
        public virtual Domain.Enumerations.WoundClassification Classification { get; set; }
        public virtual DateTime? ResolvedOn { get; set; }
        public virtual Room Room { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual DateTime? FirstNotedOn { get; set; }
        public virtual bool? IsResolved { get; set; }
        public virtual WoundStage CurrentStage { get; set; }
        public virtual WoundSite Site { get; set; }
        public virtual DateTime? LastSynchronizedAt { get; set; }
        public virtual WoundType WoundType { get; set; }
        public virtual string AdditionalSiteDetails { get; set; }
        
        public virtual IList<WoundAssessment> Assessments { get; set; }


        public WoundReport()
        {
        }

        public WoundReport(Patient patient)
        {
            Patient = patient.ThrowIfNullArgument("patient");
            Guid = GuidHelper.NewGuid();
            Assessments = new List<WoundAssessment>();
        }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Patient.Account.Id;
        }

        public virtual void EvaluateCurrentStage()
        {
            if (Assessments != null && Assessments.Count() > 0)
            {
                var lastAssessment = Assessments.OrderBy(x => x.AssessmentDate).Last();
                this.CurrentStage = lastAssessment.Stage;
            }
            else{
                this.CurrentStage = null;
            }

        }


        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<WoundReport>()
            .CreateFlag(Enumerations.AuditEntryType.AddedWound)
            .EditFlag(Enumerations.AuditEntryType.ModifiedWound)
            .RemoveFlag(Enumerations.AuditEntryType.RemovedWound)
            .RemoveExpression(x => x.Deleted == true)
            .PatientGuid(x => x.Patient.Guid)
            .ComponentGuid(x => x.Guid)
            .Ignore(x => x.Deleted)
            .Ignore(x => x.LastSynchronizedAt)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.Assessments)
            .GetDefinition();
        }

    }
}
