using System;
using System.Collections.Generic;
using System.Linq;



namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class Facility : BaseReportingEntity
    {
        public virtual string Name { get; set; }
        public virtual Account Account { get; set; }
        public virtual bool? HasSingleFloorMap { get; set; }
        public virtual List<Floor> Floors { get; set; }
    }
}
