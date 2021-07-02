using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionTreatment
    {
        public int? Id { get; set; }
        public bool Removed { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string DeliveryMethod { get; set; }
        public string SpecialInstructions { get; set; }
        public string Units { get; set; }
        public string TreatmentName { get; set; }
        public string AdministeredOn { get; set; }
        public string MDName { get; set; }
        public string DiscontinuedOn { get; set; }
    }
}
