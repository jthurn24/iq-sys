using System;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentInfo
    {
        public int Id { get; set; }

        public string PatientFullName { get; set; }

        public string PatientRoomWingName { get; set; }

        public string PatientRoomName { get; set; }

        public string IncidentTypes { get; set; }

        public string InjuryLevel { get; set; }

        public virtual DateTime? DiscoveredOn { get; set; }

        public virtual DateTime? OccurredOn { get; set; }

        public string RoomAndWingName
        {
            get { return "{0} {1}".FormatWith(PatientRoomName, PatientRoomWingName); }
        }
    }
}
