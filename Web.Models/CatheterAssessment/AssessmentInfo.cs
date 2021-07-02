using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Web.Models.CatheterAssessment
{
    public class AssessmentInfo
    {
        public string Id { get; set; }
        public string AssessmentDate { get; set; }
        public string NextAssessmentDate { get; set; }
        public string RoomWingName { get; set; }
    }
}
