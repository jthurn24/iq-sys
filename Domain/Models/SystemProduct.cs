using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class SystemProduct : Entity<SystemProduct>
    {
        public virtual string Name { get; protected set; }
        public virtual Decimal DefaultFee { get; set; }
        public virtual Enumerations.ProductFeeType DefaultFeeType { get; set; }
    }
}
