using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;
namespace IQI.Intuition.Domain.Models
{
    public class SystemContact : Entity<SystemContact>
    {
        public virtual Account Account { get; set; }
        public virtual SystemLead SystemLead { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Title { get; set; }
        public virtual string Cell { get; set; }
        public virtual string Direct { get; set; }
        public virtual string Email { get; set; }
        public virtual string Notes { get; set; }
    }
}
