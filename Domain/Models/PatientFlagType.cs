using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class PatientFlagType : Entity<PatientFlagType>
    {
        public virtual string Name { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual IEnumerable<Patient> Patients { get; set;}
    }
}
