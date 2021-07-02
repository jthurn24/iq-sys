using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class FacilityProduct : Entity<FacilityProduct>
    {
        public virtual Facility Facility { get; set; }
        public virtual Decimal Fee { get; set; }
        public virtual Enumerations.ProductFeeType FeeType { get; set; }
        public virtual SystemProduct SystemProduct { get; set; }
        public virtual DateTime? StartOn { get; set; }
        public virtual string Note { get; set; }
    }
}
