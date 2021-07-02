using System;
using System.Collections;

namespace IQI.Intuition.Web.Models.Patient
{
    public class PatientFormClientData
    {
        public IEnumerable Floors { get; set; }

        public IEnumerable Wings { get; set; }

        public IEnumerable Rooms { get; set; }

        public string CurrentStatus { get; set; }

        public string NewStatus { get; set; }

    }
}
