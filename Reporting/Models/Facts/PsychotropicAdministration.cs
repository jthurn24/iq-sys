using System;
using System.Collections.Generic;
using System.Linq;

using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Reporting.Models.Facts
{
    public class PsychotropicAdministration : BaseReportingEntity
    {
        public virtual Account Account { get; set; }
        public virtual Facility Facility { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual PsychotropicDrugName DrugName { get; set; }
        public virtual PsychotropicDrugType DrugType { get; set; }
        public List<PsychotropicAdministrationMonth> AdministrationMonths { get; set; }
    }
}
