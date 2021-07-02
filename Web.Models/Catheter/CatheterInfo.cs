using System;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Models.Catheter
{
    public class CatheterInfo
    {
        public int Id { get; set; }

        public Guid PatientGuid { get; set; }
        public string PatientFullName { get; set; }

        public string PatientRoomWingName { get; set; }

        public string PatientRoomName { get; set; }

        public string Diagnosis { get; set; }

        public virtual DateTime? StartedOn { get; set; }

        public virtual DateTime? DiscontinuedOn { get; set; }

        public string Type { get; set; }
        public string Material { get; set; }
        public string Reason { get; set; }

        public string RoomAndWingName
        {
            get { return "{0} {1}".FormatWith(PatientRoomName, PatientRoomWingName); }
        }
    }
}
