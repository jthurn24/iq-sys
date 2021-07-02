using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class FloorMapRoomIncidentType : BaseReportingEntity
    {

        public virtual FloorMap FloorMap { get; set; }
        public virtual List<RoomEntry> RoomEntries { get; set; }

        public class RoomEntry
        {
            public virtual FloorMapRoom FloorMapRoom { get; set; }
            public List<EntityEntry> EntityEntries { get; set; }
        }

        public class EntityEntry
        {
            public virtual List<IncidentType> IncidentTypes { get; set; }
            public virtual List<IncidentTypeGroup> IncidentTypeGroups { get; set; }
            public Guid Component { get; set; }
            public DateTime Date { get; set; }
            public FloorMapRoom FloorMapRoom { get; set; }
        }
    }
}
