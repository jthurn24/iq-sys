using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class WarningRuleDefault : Entity<WarningRuleDefault>
    {
        public virtual string Description { get; set; }
        public virtual string Title { get; set; }
        public virtual string Arguments { get; set; }
        public virtual string TypeName { get; set; }
        public virtual string ItemTemplate { get; set; }
    }
}
