using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class LabResult : Entity<LabResult>
    {
        public virtual string Name { get; set; }
        public virtual bool Positive { get; set; }
        public virtual LabResultSet LabResultSet { get; set; }
    }
}
