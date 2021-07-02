using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Configuration.Wing
{
    public class WingEntry
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? FloorId { get; set; }
        public Guid Guid { get; set; }
    }
}
