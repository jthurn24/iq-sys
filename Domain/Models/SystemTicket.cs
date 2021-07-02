using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class SystemTicket : Entity<SystemTicket>
    {
        public virtual SystemTicketType SystemTicketType { get; set; }
        public virtual Enumerations.SystemTicketStatus Status { get; set; }
        public virtual Account Account { get; set; }
        public virtual AccountUser AccountUser { get; set; }
        public virtual string Details { get; set; }
        public virtual int Priority { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual DateTime? ClosedOn { get; set; }
        public virtual string Release { get; set; }
    }
}
