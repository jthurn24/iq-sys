using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel;
using RedArrow.Framework.Utilities;

namespace IQI.Intuition.Domain.Models
{
    public class ResourceFolder : Entity<ResourceFolder>
    {
        public virtual string Name { get; set; }
        public virtual ResourceFolder Parent { get; set; }
        public virtual IList<ResourceFolder> Children { get; set; }
        public virtual IList<Resource> Resources { get; set; }

        public virtual string GetFullPath()
        {
            string path = this.Name;

            var parent = this.Parent;

            while (parent != null)
            {
                path = parent.Name + " / " + path;
                parent = parent.Parent;
            }

            return path;
        }
    }
}
