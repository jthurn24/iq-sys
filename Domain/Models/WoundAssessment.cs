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
    public class WoundAssessment : AuditTrackingEntity<WoundAssessment>, IRestrictable<Account>, ITrackChanges
    {
        public virtual WoundReport Report { get; set; }
        public virtual WoundStage Stage { get; set; }
        public virtual DateTime? AssessmentDate { get; set; }
        public virtual Room Room { get; set; }

        public virtual decimal? Lcm { get; set; }
        public virtual decimal? Wcm { get; set; }
        public virtual decimal? Dcm { get; set; }
        public virtual bool? Superficial { get; set; }
        public virtual int? Undermining1From { get; set; }
        public virtual int? Undermining1To { get; set; }
        public virtual decimal? Undermining1Depth { get; set; }
        public virtual int? Undermining2From { get; set; }
        public virtual int? Undermining2To { get; set; }
        public virtual decimal? Undermining2Depth { get; set; }
        public virtual int? Tunnel1Location { get; set; }
        public virtual decimal? Tunnel1Depth { get; set; }
        public virtual int? Tunnel2Location { get; set; }
        public virtual decimal? Tunnel2Depth { get; set; }
        public virtual int? Tunnel3Location { get; set; }
        public virtual decimal? Tunnel3Depth { get; set; }
        public virtual Domain.Enumerations.WoundExudate Exudate { get; set;}
        public virtual Domain.Enumerations.WoundExudateType ExudateType { get; set; }
        public virtual bool? Odor { get; set; }
        public virtual Decimal? WoundBedEpithelial { get; set; }
        public virtual Decimal? WoundBedGranulation { get; set; }
        public virtual Decimal? WoundBedSlough { get; set; }
        public virtual Decimal? WoundBedNecrosis { get; set; }
        public virtual string WoundBedOther { get; set; }
        public virtual Domain.Enumerations.WoundEdge WoundEdge { get; set; }
        public virtual Domain.Enumerations.WoundPeriwoundTissue PeriwoundTissue { get; set; }
        public virtual bool? Pain { get; set; }
        public virtual string PainManagedWith { get; set; }
        public virtual Domain.Enumerations.WoundProgress Progress { get; set; }
        public virtual int? PushScore { get; set; }
        public virtual Domain.Enumerations.WoundTreatmentStatus TreatmentStatus { get; set; }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return this.Report.Patient.Account.Id == source.Id;
        }

        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<WoundAssessment>()
            .CreateFlag(Enumerations.AuditEntryType.ModifiedWound)
            .EditFlag(Enumerations.AuditEntryType.ModifiedWound)
            .RemoveFlag(Enumerations.AuditEntryType.ModifiedWound)
            .PatientGuid(x => x.Report.Patient.Guid)
            .ComponentGuid(x => x.Report.Guid)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .GetDefinition();
        }

    }
}
