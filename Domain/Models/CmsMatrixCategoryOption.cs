using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;


namespace IQI.Intuition.Domain.Models
{
    public class CmsMatrixCategoryOption : Entity<CmsMatrixCategoryOption>
    {
        public virtual string OptionValue { get; set; }
        public virtual string OptionDescription { get; set; }
        public virtual CmsMatrixCategory Category { get; set; }
        public virtual string MDS3 { get; set; }
    }
}
