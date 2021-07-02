using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.PsychotropicDosageChange
{
    public class DosageForm
    {
        public IEnumerable<DosageSegmentEntry> Entries { get; set; }
        public string Prefix { get; set; }
    }
}
