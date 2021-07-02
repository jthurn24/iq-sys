using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PsychotropicAdministrationFormEdit
    {
        public string Id { get; set; }
        public int? DrugType { get; set; }
        public string Name { get; set; }
        public int? DosageForm { get; set; }
        public string SideEffects { get; set; }
        public string TargetBehavior { get; set; }
    }
}
