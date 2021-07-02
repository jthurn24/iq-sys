using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class DrugSection : Entity<DrugSection>
    {
        public virtual Drug Drug { get; set; }
        public virtual string SectionName { get; set; }
        public virtual string SectionText { get; set; }
        public virtual string SectionTitle { get; set; }
    }
}
