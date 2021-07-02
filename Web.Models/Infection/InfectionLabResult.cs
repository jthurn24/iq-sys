using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionLabResult
    {
        public int LabTestTypeId { get; set; }
        public int? InfectionLabResultId { get; set; }
        public string LabResultName { get; set; }
        public int LabResultId { get; set; }
        public string LabTestTypeName { get; set; }
        public string LabCompletedOn { get; set; }
        public string Notes { get; set; }
        public bool Removed { get; set; }
        public List<InfectionLabResultPathogen> Pathogens { get; set; }

        public List<Option> PathogenOptions { get; set; }
        public List<Option> PathogenQuantityOptions { get; set; }
    }
}
