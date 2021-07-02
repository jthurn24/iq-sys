using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;
using IQI.Intuition.Domain.Services.Psychotropic;

namespace IQI.Intuition.Domain.Models
{
    public class PsychotropicDosageChange : AuditTrackingEntity<PsychotropicDosageChange>, IRestrictable<Account>
    {
        public virtual PsychotropicAdministration Administration { get; set; }
        public virtual decimal? Dosage { get; set; }
        public virtual PsychotropicFrequency Frequency { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual string DosageSegments { get; set; }


        public virtual decimal? GetDailyAverageDosage()
        {
            return this.Frequency.GetFrequencyDefinition().GetDailyAverage(this);
        }

        public virtual decimal? GetTotalDosage()
        {
            return this.Frequency.GetFrequencyDefinition().GetTotal(this);
        }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Administration.Patient.Account.Id;
        }

    }
}
