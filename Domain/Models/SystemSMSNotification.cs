using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class SystemSMSNotification : Entity<SystemSMSNotification>
    {
        public virtual string SendTo { get; set; }
        public virtual string Message { get; set; }
        public virtual bool? AllowAfterHours { get; set; }
    }
}
