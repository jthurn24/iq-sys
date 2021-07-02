using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class LabTestType : Entity<LabTestType>
    {
        public virtual string Name { get; set; }
        public virtual LabResultSet LabResultSet { get; set; }
        public virtual PathogenSet PathogenSet { get; set; }
        public virtual PathogenQuantitySet PathogenQuantitySet { get; set; }
    }
}
