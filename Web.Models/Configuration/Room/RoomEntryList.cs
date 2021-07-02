using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Configuration.Room
{
    public class RoomEntryList
    {
        public IList<RoomEntry> Entries { get; set; }
        public string WingName { get; set; }
        public int WingId { get; set; }
        public RoomEntry NewRoom { get; set; }
    }
}
