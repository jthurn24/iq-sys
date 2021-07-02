using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Reporting.Patient
{
    public class PatientListView
    {

        public int? SortMode { get; set; }
        public List<SelectListItem> SortModeOptions { get; set; }
        public bool ShowDetails { get; set; }

        public List<Patient> Patients { get; set; }

        public class Patient
        {
            public string AdmissionDate { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Middle { get; set; }
            public string Floor { get; set; }
            public string Wing { get; set; }
            public string Room { get; set; }

            public IEnumerable<string> Flags { get; set; }
            public IEnumerable<string> Precautions { get; set; }
            public IEnumerable<string> Warnings { get; set; }
        }
    }
}
