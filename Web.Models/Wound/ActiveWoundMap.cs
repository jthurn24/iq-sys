using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Wound
{
    public class ActiveWoundMap
    {
        public BodyGraph Graph { get; set; }
        public IList<WoundSite> Sites { get; set; }
    }
}
