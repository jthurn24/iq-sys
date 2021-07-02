using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Reporting.Composite
{
    [Serializable]
    public class CompositeRequest
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public bool Landscape { get; set; }
    }
}
