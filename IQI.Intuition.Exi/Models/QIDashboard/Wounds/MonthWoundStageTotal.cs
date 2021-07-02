using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models;

namespace IQI.Intuition.Exi.Models.QIDashboard.Wounds
{
    public class MonthWoundStageTotal : AnnotatedEntry
    {
        public WoundStage Stage { get; set; }
        public int Total { get; set; }
        public Decimal Rate { get; set; }
        public Month Month { get; set; }
        public Decimal Change { get; set; }
        public int CensusPatientDays { get; set; }
    }
}
