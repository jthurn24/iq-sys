using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Cubes
{
    public class FloorMapRoomWoundStage : BaseReportingEntity
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
            public WoundStage WoundStage { get; set; }
            public Guid Component { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public FloorMapRoom FloorMapRoom { get; set; }
        }

    }
}
