using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Infection
{
    public class RemoveInfection
    {
        public IEnumerable<SelectListItem> PatientOptions { get; set; }
        public int? Patient { get; set; }
        public IEnumerable<SelectListItem> PatientInfectionOptions { get; set; }
    }
}
