using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionNote
    {
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Note { get; set; }
        public int? InfectionNoteId { get; set; }
        public bool Removed { get; set; }

    }
}
