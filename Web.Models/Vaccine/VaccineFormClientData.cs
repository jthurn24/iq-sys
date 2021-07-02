using System;
using System.Collections;

namespace IQI.Intuition.Web.Models.Vaccine
{
    public class VaccineFormClientData
    {
        public IEnumerable Floors { get; set; }

        public IEnumerable Wings { get; set; }

        public IEnumerable Rooms { get; set; }
    }
}
