using System;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Models.Patient
{
    public class PatientInfo
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string RoomWingFloorName { get; set; }

        public string RoomWingName { get; set; }

        public string RoomName { get; set; }

        public PatientInfoStatus? Status { get; set; }

        public string WingName
        {
            get { return "{0} Floor - {1}".FormatWith(RoomWingFloorName, RoomWingName); }
        }
    }
}
