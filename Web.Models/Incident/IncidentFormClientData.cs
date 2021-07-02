using System;
using System.Collections;

namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentFormClientData
    {
        public IEnumerable Floors { get; set; }

        public IEnumerable Wings { get; set; }

        public IEnumerable Rooms { get; set; }
    }
}
