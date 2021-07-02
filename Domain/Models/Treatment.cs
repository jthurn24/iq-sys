using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class Treatment : Entity<Treatment>
    {
        public virtual string Name { get; set; }
        public virtual TreatmentType TreatmentType { get; set; }
    }
}
