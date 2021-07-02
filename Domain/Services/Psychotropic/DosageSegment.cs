using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Services.Psychotropic;

namespace IQI.Intuition.Domain.Services.Psychotropic
{
    public class DosageSegment
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public decimal? Dosage { get; set; }
        public List<string> DescriptionOptions { get; set; }
    }
}
