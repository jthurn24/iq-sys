using System;
using System.Collections.Generic;
using System.Linq;



namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class FloorMapRoom : BaseReportingEntity
    {
        public virtual FloorMap FloorMap {get; set;}
        public virtual Room Room { get; set; }
        public virtual string Coordinates { get; set; }
    }
}
