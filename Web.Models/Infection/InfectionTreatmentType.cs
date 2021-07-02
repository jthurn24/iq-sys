using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionTreatmentType
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public IList<InfectionTreatment> Treatments { get; set; }
        public IList<string> TreatmentNames { get; set; }
    }
}
