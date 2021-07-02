using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;

namespace IQI.Intuition.Domain.Models
{
    public class PsychotropicAdministrationPRN : AuditTrackingEntity<PsychotropicAdministrationPRN>, IRestrictable<Account>
    {
        public virtual PsychotropicAdministration Administration { get; set; }
        public virtual decimal? Dosage { get; set; }
        public virtual DateTime? GivenOn { get; set; }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Administration.Patient.Account.Id;
        }

    }
}
