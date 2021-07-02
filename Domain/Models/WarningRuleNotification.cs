using System;
using System.Collections.Generic;
using System.Linq;
using IQI.Intuition.Domain.Utilities;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Encryption;


namespace IQI.Intuition.Domain.Models
{
    public class WarningRuleNotification : Entity<WarningRuleNotification>
    {
        public virtual AccountUser AccountUser { get; set; }
        public virtual WarningRule WarningRule { get; set; }
    }
}
