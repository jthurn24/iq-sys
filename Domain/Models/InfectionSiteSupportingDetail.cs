using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionSiteSupportingDetail : AuditTrackingEntity<InfectionSiteSupportingDetail>
    {
        protected InfectionSiteSupportingDetail() { }

        public InfectionSiteSupportingDetail(InfectionSite infectionSite, string name)
        {
            this.InfectionSite = infectionSite.ThrowIfNullArgument("infectionSite");
            this.Name = name.ThrowIfNullOrWhitespaceArgument("name");
        }

        public virtual InfectionSite InfectionSite { get; protected set; }

        public virtual string Name { get; protected set; }
    }
}
