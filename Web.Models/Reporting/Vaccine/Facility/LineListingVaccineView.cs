using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Models.Reporting.Vaccine.Facility
{
    public class LineListingVaccineView
    {
        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public IEnumerable<SelectListItem> TypeOptions { get; set; }
        public IEnumerable<SelectListItem> SortOptions { get; set; }

        public int? VaccineTypeId { get; set; }
        public int? SortId { get; set; }

        public IEnumerable<SelectListItem> RefusalOptions { get; set; }
        public int? VaccineRefusalReasonId { get; set; }

        public int? Wing { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeDischarged { get; set; }

        public IList<PatientRow> Patients { get; set; }

        public void SetData(IEnumerable<VaccineEntry> vaccineData, IEnumerable<IQI.Intuition.Domain.Models.Patient> patients)
        {
            this.Patients = patients.Select(x => new PatientRow()
            {
                PatientName = x.FullName,
                Room = x.Room.Name,
                Wing = x.Room.Wing.Name,
                Floor = x.Room.Wing.Floor.Name,
                AdmissionDate = x.GetLastAdmissionDate().FormatAsShortDate(),
                ID = x.Id.ToString(),
                PatientBirthDate = x.BirthDate.Value.ToShortDateString(),
                PatientGuid = x.Guid,
                Entries = new List<VacineRowEntry>()
            })
            .ToList();

            foreach(var v in vaccineData)
            {
                var p = this.Patients.Where(x => x.PatientGuid == v.Patient.Guid).FirstOrDefault();

                if(p != null)
                {
                    p.Entries.Add(new VacineRowEntry()
                    {
                         AdministeredOn = v.AdministeredOn.Value.ToShortDateString(),
                         RefusalReason = v.VaccineRefusalReason != null ? v.VaccineRefusalReason.CodeValue: String.Empty,
                         Refused = v.VaccineRefusalReason != null,
                         VaccineGuid = v.Guid,
                         VaccineType = v.VaccineType.CVXShortDescription,
                         Id = v.Id.ToString()
                    });
                }
            }
            
        }

        public class PatientRow
        {
            public string ID { get; set; }

            public string PatientName { get; set; }
            public string Room { get; set; }
            public string Wing { get; set; }
            public string Floor { get; set; }
            public string AdmissionDate { get; set; }
            public string PatientBirthDate { get; set; }
            public Guid PatientGuid { get; set; }
            public List<VacineRowEntry> Entries { get; set; }


            public void VaccineRow(VaccineEntry vaccine)
            {
                PatientName = vaccine.Patient.FullName;
                Room = vaccine.Patient.Room.Name;
                Wing = vaccine.Patient.Room.Wing.Name;
                Floor = vaccine.Patient.Room.Wing.Floor.Name;
                PatientBirthDate = vaccine.Patient.BirthDate.FormatAs("MM/dd/yyyy");        
                ID = vaccine.Id.ToString();

                PatientGuid = vaccine.Patient.Guid;

                var lastAdmitDate = vaccine.Patient.GetLastAdmissionDate();
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

        public class VacineRowEntry
        {
            public Guid VaccineGuid { get; set; }
            public string AdministeredOn { get; set; }
            public bool Refused { get; set; }
            public string VaccineType { get; set; }
            public string RefusalReason { get; set; }
            public string Id { get; set; }
        }
    }
}
