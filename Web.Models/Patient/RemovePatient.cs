using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Patient
{
    public class RemovePatient
    {
        public string PatientName { get; set; }
        public int? Patient { get; set; }
        public bool MoveData { get; set; }
        public IEnumerable<SelectListItem> DestinationPatientOptions { get; set; }
        public int? DestinationPatient { get; set; }
    }
}
