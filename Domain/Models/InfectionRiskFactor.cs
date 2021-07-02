using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionRiskFactor : AuditTrackingEntity<InfectionRiskFactor>
    {
        protected InfectionRiskFactor() { }

        public InfectionRiskFactor(string name)
        {
            Name = name.ThrowIfNullOrWhitespaceArgument("name");
        }

        public virtual string Name { get; protected set; }

        public virtual IEnumerable<InfectionVerification> InfectionVerifications { get; protected set; } // Many-to-many reference
    }
}
