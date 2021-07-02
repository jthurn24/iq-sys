using System;
using System.Collections;
using System.Collections.Generic;

namespace IQI.Intuition.Web.Models.CatheterAssessment
{
    public class AssessmentFormClientData
    {
        public IEnumerable Floors { get; set; }

        public IEnumerable Wings { get; set; }

        public IEnumerable Rooms { get; set; }
    }
}
