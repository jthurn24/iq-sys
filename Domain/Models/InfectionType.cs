using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionType : AuditTrackingEntity<InfectionType>
    {

        protected InfectionType() { }

        public InfectionType(string name)
        {
            this.Name = name.ThrowIfNullOrWhitespaceArgument("name");
        }

        public virtual string Name { get;  set; }
        public virtual int SortOrder { get; set; }
        public virtual bool? IsHidden { get; set; }
        public virtual string Color { get; set; }
        public virtual InfectionDefinition Definition { get; set; }
        public virtual bool? UsedForEmployees { get; set; }
        public virtual string ShortName { get; set; }
    }
}
