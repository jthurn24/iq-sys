using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.Patient;

namespace IQI.Intuition.Web.Models.Precaution
{
    public class PatientPrecautionForm
    {

        public Guid? Guid { get; set; }
        public int? PrecautionTypeId { get; set; }
        public int? ProductId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool Ended { get; set; }
        public int PatientId { get; set; }
        public string AdditionalDescription { get; set; }
    }
}
