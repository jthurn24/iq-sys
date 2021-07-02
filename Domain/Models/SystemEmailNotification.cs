using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class SystemEmailNotification : Entity<SystemEmailNotification>
    {
        public virtual string SendTo { get; set; }
        public virtual string Subject { get; set; }
        public virtual string MessageText { get; set; }
        public virtual bool? IsHtml { get; set; }
    }
}
