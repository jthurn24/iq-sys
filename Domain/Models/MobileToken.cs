using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class MobileToken : Entity<MobileToken>
    {
        public virtual string Token { get; set; }
        public virtual int AccountUserId { get; set; }
        public virtual DateTime CreatedOn { get; set; }
    }
}
