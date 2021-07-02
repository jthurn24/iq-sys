using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class WoundStage : Entity<WoundStage>
    {
        public virtual string Name { get; set; }
        public virtual int? RatingValue { get; set; }
    }
}
