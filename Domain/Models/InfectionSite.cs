using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionSite : AuditTrackingEntity<InfectionSite>
    {
        protected InfectionSite() { }

        public InfectionSite(string name, InfectionType type)
        {
            Name = name.ThrowIfNullOrWhitespaceArgument("name");
            Type = type.ThrowIfNullArgument("type");
        }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string DescriptionOrName { get { return Description ?? Name; } }

        public virtual InfectionType Type { get; protected set; }

        public virtual string SupportingDetailsDescription { get; set; }

        public virtual IEnumerable<InfectionSiteSupportingDetail> SupportingDetails { get; protected set; }

        public virtual InfectionCriteriaRuleSet RuleSet { get; set; }

        public virtual Enumerations.InfectionCriteriaAvailabilityRule CriteriaAvailabilityRule { get; set; }

    }
}
