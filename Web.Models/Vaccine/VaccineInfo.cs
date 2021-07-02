using System;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Domain.Models;

namespace IQI.Intuition.Web.Models.Vaccine
{
    public class VaccineInfo
    {
        public int Id { get; set; }

        public string PatientFullName { get; set; }

        public string PatientRoomWingName { get; set; }

        public string PatientRoomName { get; set; }

        public string VaccineType { get; set; }

        public DateTime AdministeredOn { get; set; }

        public string RefusalReason { get; set; }

        public string RoomAndWingName
        {
            get { return "{0} {1}".FormatWith(PatientRoomName, PatientRoomWingName); }
        }
    }
}
