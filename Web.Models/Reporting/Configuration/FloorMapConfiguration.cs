using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Reporting.Configuration
{
    public class FloorMapConfiguration
    {
        public List<FloorMapConfigurationPoint> Points { get; set; }
        public IEnumerable<SelectListItem> FloorMapOptions { get; set; }
        public Guid? SelectedFloorMap { get; set; }

    }
}
