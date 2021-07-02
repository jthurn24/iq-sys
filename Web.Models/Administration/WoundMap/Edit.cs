using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using System.Web.Mvc;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using System.Drawing;

namespace IQI.Intuition.Web.Models.Administration.WoundMap
{
    public class Edit
    {
        public BodyGraph Graph { get; set; }
        public IList<SelectListItem> Sites { get; set; }
    }
}
