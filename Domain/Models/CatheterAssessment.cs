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
    public class CatheterAssessment : AuditTrackingEntity<CatheterAssessment> //, IRestrictable<Account>, ITrackChanges
    {
        public virtual Patient Patient { get; set; }
        public virtual Room Room { get; set; }
        public virtual CatheterEntry CatheterEntry { get; set; }

        public virtual int? ContinuedUseEstimateDays { get; set; }
        public virtual int? Action { get; set; }

        public virtual bool? TubeHolderUsed { get; set; }
        
        public virtual string TherapyGoal { get; set; }
        public virtual string AlternativeInterventions { get; set; }
        public virtual string CSResults { get; set; }
        public virtual string ReversibleFactors { get; set; }
        
        public virtual DateTime? NextAssessmentDate { get; set; }
        public virtual DateTime? AssessmentDate { get; set; }

        public virtual string RemovalReason { get; set; }
        public virtual int? RemovalPvr1 { get; set; }
        public virtual int? RemovalPvr1Hours { get; set; }
        public virtual int? RemovalPvr2 { get; set; }
        public virtual int? RemovalPvr2Hours { get; set; }
        public virtual int? RemovalPvr3 { get; set; }
        public virtual int? RemovalPvr3Hours { get; set; }

        public virtual string RemovalComplications { get; set; }
        public virtual string RemoveAlternativeTherapeutic { get; set; }
        public virtual int? RemoveIntakeDaily { get; set; }
        public virtual string RemoveQualityOfUrine { get; set; }
        public virtual string RemoveResidentResponse { get; set; }
        public virtual bool? RemovedAndReplaced { get; set; }
        public virtual string RemovedAndReplacedReason { get; set; }
    }
}
