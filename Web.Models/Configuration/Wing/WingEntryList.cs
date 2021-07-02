using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Configuration.Wing
{
    public class WingEntryList
    {
        public IList<WingEntry> Entries { get; set; }
        public string FloorName { get; set; }
        public int FloorId { get; set; }
    }
}
