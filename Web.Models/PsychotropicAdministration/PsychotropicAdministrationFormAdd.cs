using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.PsychotropicDosageChange;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PsychotropicAdministrationFormAdd
    {
        public int? DrugType { get; set; }
        public string Name { get; set; }
        public int? DosageForm { get; set; }
        public string SideEffects { get; set; }
        public DateTime? StartDate { get; set; }
        public int? Frequency { get; set; }
        public string TargetBehavior { get; set; }
        public IEnumerable<DosageSegmentEntry> Segments { get; set; }

        public DosageForm GetSegmentForm()
        {
            return new DosageForm()
            {
                Entries = this.Segments,
                Prefix = "Segments"
            };
        }
    }
}
