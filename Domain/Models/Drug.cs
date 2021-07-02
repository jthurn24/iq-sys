using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class Drug : Entity<Drug>
    {
        public virtual Domain.Enumerations.DrugType DrugType { get; set; }
        public virtual string Name { get; set; }
        public virtual string SourceIdentifier { get; set; }
        public virtual IList<DrugSection> Sections { get; set; }
    }
}
