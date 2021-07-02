using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentWitness
    {
        public string Name { get; set; }
        public string Statement { get; set; }
        public string Role { get; set; }
        public bool Removed { get; set; }
        public int? IncidentWitnessId { get; set; }
    }
}
