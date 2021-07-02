using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Configuration.Room
{
    public class RoomEntry
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? WingId { get; set; }
        public Guid Guid { get; set; }
        public bool? IsInactive { get; set; }
    }
}
