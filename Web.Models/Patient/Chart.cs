using System;
using System.Collections.Generic;

namespace IQI.Intuition.Web.Models.Patient
{
    public class Chart : PatientInfo
    {
        public string MDName { get; set; }

        public IEnumerable<string> Warnings { get; set; }

        public IEnumerable<ChartStatusChange> StatusChanges { get; set; }

        public IEnumerable<ChartRoomChange> RoomChanges { get; set; }

        public IEnumerable<string> Flags { get; set; }
    }
}
