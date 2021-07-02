using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.PsychotropicDosageChange
{
    public class DosageSegmentEntry
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public decimal? Dosage { get; set; }
        public List<string> DescriptionOptions { get; set; }
    }
}
