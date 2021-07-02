using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;

namespace IQI.Intuition.Web.Models.Reporting.Catheter.Facility
{
    public class LineListingCatheterView
    {

        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? Wing { get; set; }
        public IList<CatheterRow> Catheters { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeDiscontinued { get; set; }

        public void SetData(IEnumerable<CatheterEntry> cData)
        {
            this.Catheters = new List<CatheterRow>();

            foreach (var c in cData)
            {
                var row = new CatheterRow(c);
                this.Catheters.Add(row);
            }
        }

        public class CatheterRow
        {
            public string PatientName { get; set; }
            public string Date { get; set; }
            public string Room { get; set; }
            public string Wing { get; set; }
            public string Floor { get; set; }
            public string AdmissionDate { get; set; }
            public string ID { get; set; }
            public string PatientBirthDate { get; set; }
            public string StartedOn { get; set; }
            public string DiscontinuedOn { get; set; }
            public string Diagnosis { get; set; }
            public string InitialRoom { get; set; }
            public string InitialWing { get; set; }
            public string InitialFloor { get; set; }

            public CatheterRow(CatheterEntry entry)
            {
                PatientName = entry.Patient.FullName;
                Room = entry.Patient.Room.Name;
                Wing = entry.Patient.Room.Wing.Name;
                Floor = entry.Patient.Room.Wing.Floor.Name;
                PatientBirthDate = entry.Patient.BirthDate.FormatAs("MM/dd/yyyy");
                ID = entry.Id.ToString();
                StartedOn = entry.StartedOn.FormatAs("MM/dd/yyyy");
                DiscontinuedOn = entry.DiscontinuedOn.FormatAs("MM/dd/yyyy");
                Diagnosis = entry.Diagnosis;

                InitialRoom = entry.Room.Name;
                InitialWing = entry.Room.Wing.Name;
                InitialFloor = entry.Room.Wing.Floor.Name;

                var lastAdmitDate = entry.Patient.GetLastAdmissionDate();

                if (lastAdmitDate == null)
                {
                    AdmissionDate = string.Empty;
                }
                else
                {
                    AdmissionDate = lastAdmitDate.Value.ToString("MM/dd/yy");
                }

               
            }
        }
    }
}
