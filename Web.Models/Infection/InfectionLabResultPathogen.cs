using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionLabResultPathogen
    {
        public int PathogenId { get; set; }
        public string PathogenName { get; set; }
        public int? PathogenQuantityId { get; set; }
        public string PathogenQuantityName { get; set; }

        public int? Id { get; set; }
        public bool Removed { get; set; }
    }
}
