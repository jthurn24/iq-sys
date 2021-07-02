using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class Resource : Entity<ResourceFolder>
    {
        public virtual Enumerations.ResourceType ResourceType { get; set; }
        public virtual ResourceFolder Folder { get; set; }
        public virtual string Description { get; set; }
        public virtual string Link { get; set; }
        public virtual AccountUser SuggestedBy { get; set; }
        public virtual string Name { get; set; }
        public virtual string FileExtension { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
    }
}
