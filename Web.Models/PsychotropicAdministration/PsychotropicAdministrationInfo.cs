using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.PsychotropicAdministration
{
    public class PsychotropicAdministrationInfo
    {
        public string PatientGuid { get; set; }
        public string Id { get; set; }
        public string DrugTypeName { get; set; }
        public string Name { get; set; }
        public string CurrentDosage { get; set; }
        public string PreviousDosage { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Active { get; set; }
        public string SideEffects { get; set; }
        public string TargetBehavior { get; set; }
        public string PatientFullName { get; set; }
        public string PatientRoomWingName { get; set; }

    }
}
