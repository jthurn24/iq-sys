using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class CmsMatrixCategory : Entity<CmsMatrixCategory>
    {
        public virtual string Name { get; set; }
        public virtual bool? Editable { get; set; }
        public virtual bool? Active { get; set; }
        public virtual int? ColumnNumber { get; set; }
        public virtual string DescriptionText { get; set; }
        public virtual IList<CmsMatrixCategoryOption> Options { get; set; }
        public virtual CmsMatrixType MatrixType { get; set; }
    }
}
