using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Resource
{
    public class ResourceEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public Domain.Enumerations.ResourceType ResourceType { get; set; }
        public string SuggestedBy { get; set; }
        public string Link { get; set; }
        public string CreatedOn { get; set; }
    }
}
