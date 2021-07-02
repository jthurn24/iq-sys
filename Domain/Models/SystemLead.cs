using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class SystemLead : Entity<SystemTicket>
    {
        public virtual Enumerations.SystemLeadStatus Status { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual string Name { get; set; }
        public virtual string State { get; set; }
        public virtual string City { get; set; }
        public virtual string Source { get; set; }
        public virtual string Details { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual DateTime? LastContactedOn { get; set;  }
        public virtual DateTime? LastDemoOn { get; set; }
        public virtual Enumerations.SystemLeadMailingStatus MailingStatus { get; set; }
        public virtual string Phone { get; set; }
        public virtual int Beds { get; set; }
        public virtual string TypeOfOwnership { get; set; }
        public virtual string MultiHome { get; set; }
        public virtual Enumerations.FacilityType FacilityType { get; set; }
        public virtual string ProviderNumber { get; set; }

    }
}
