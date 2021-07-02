using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;

namespace IQI.Intuition.Domain.Models
{
    public class PsychotropicAdministration :AuditTrackingEntity<PsychotropicAdministration>, IRestrictable<Account>
    {
        public virtual Patient Patient { get; set; }
        public virtual PsychotropicDrugType DrugType { get; set; }
        public virtual string Name { get; set; }
        public virtual PsychotropicDosageForm DosageForm { get; set; }
        public virtual string SideEffects { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual bool? Active { get; set; }
        public virtual string TargetBehavior { get; set; }
        public virtual DateTime? LastSynchronizedAt { get; set; }
        public virtual Guid Guid { get; set; }

        public virtual IList<PsychotropicDosageChange> DosageChanges { get; set; }
        public virtual IList<PsychotropicAdministrationPRN> PRNs { get; set; }

        public PsychotropicAdministration()
        {
        }

        public PsychotropicAdministration(Patient patient)
        {
            Patient = patient.ThrowIfNullArgument("patient");
            Guid = GuidHelper.NewGuid();
            DosageChanges = new List<PsychotropicDosageChange>();
            PRNs = new List<PsychotropicAdministrationPRN>();
        }


        public virtual void EvaluateActive()
        {
            if (DosageChanges == null || DosageChanges.Count() < 1)
            {
                Active = true;
                return;
            }

            Active = DosageChanges.OrderBy(x => x.StartDate).Last().Frequency.GetFrequencyDefinition().IndicatesActiveAdministration();

        }

        public virtual DateTime? GetStartDate()
        {
            if (this.DosageChanges == null || DosageChanges.Count() < 1)
            {
                return null;
            }

           return this.DosageChanges.OrderByDescending(x => x.StartDate).Last().StartDate;
        }

        public virtual DateTime? GetEndDate()
        {
            if (this.DosageChanges == null || DosageChanges.Count() < 1)
            {
                return null;
            }

            var last = this.DosageChanges.OrderByDescending(x => x.StartDate).First();

            if (last.Frequency.GetFrequencyDefinition().IndicatesActiveAdministration())
            {
                return null;
            }

            return last.StartDate;
        }


        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Patient.Account.Id;
        }

    }
}
