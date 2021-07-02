using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class PrecautionType : Entity<PrecautionType>
    {
        public virtual SystemProduct SystemProduct { get; set; }
        public virtual string Name { get; set; }
        public virtual string SubProductTypeKey { get; set; }
    }
}
