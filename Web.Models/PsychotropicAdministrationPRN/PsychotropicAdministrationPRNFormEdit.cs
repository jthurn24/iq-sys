using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.PsychotropicAdministrationPRN
{
    public class PsychotropicAdministrationPRNFormEdit
    {
        public string Id { get; set; }
        public decimal? Dosage { get; set; }
        public DateTime? GivenOn { get; set; }
    }
}
