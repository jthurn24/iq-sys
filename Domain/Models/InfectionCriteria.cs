using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionCriteria : AuditTrackingEntity<InfectionCriteria>
    {
        protected InfectionCriteria() { }

        public InfectionCriteria(InfectionCriteriaRule rule, string name)
        {
            Rule = rule.ThrowIfNullArgument("rule");
            Name = name.ThrowIfNullOrWhitespaceArgument("name");
        }

        public virtual InfectionCriteriaRule Rule { get; set; }

        public virtual string Name { get;  set; }

        public virtual IEnumerable<InfectionVerification> InfectionVerifications { get; protected set; } // Many-to-many reference
    }
}
