using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Resource
{
    public class ResourceEntryList
    {
        public string Path { get; set; }
        public int FolderId { get; set; }
        public List<ResourceEntry> Entries { get; set; }
    }
}
