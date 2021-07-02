using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Reporting.Models.Dimensions
{
    public class FloorMapImage : BaseReportingEntity
    {
        public byte[] Image { get; set; }
        public FloorMap FloorMap { get; set; }
    }
}
