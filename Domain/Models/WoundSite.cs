using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class WoundSite : Entity<WoundSite>
    {
        public virtual string Name { get; set; }
        public virtual int? TopLeftX { get; set; }
        public virtual int? TopLeftY { get; set; }
        public virtual int? BottomRightX { get; set; }
        public virtual int? BottomRightY { get; set; }
        public virtual int? Priority { get; set; }
    }
}
