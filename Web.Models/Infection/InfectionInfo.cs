using System;
using RedArrow.Framework.Extensions.Common;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionInfo
    {
        public int Id { get; set; }

        public string PatientFullName { get; set; }

        public string RoomWingName { get; set; }

        public string RoomName { get; set; }

        public string InfectionSiteTypeName { get; set; }

        public DateTime? FirstNotedOn { get; set; }

        public string ReasonForEntry { get; set; }


        public string LabResultsText { get; set; }

        public string TherapyText { get; set; }


        public string ResolvedOn { get; set; }

        public string RoomAndWingName
        {
            get { return "{0} {1}".FormatWith(RoomName, RoomWingName); }
        }
    }
}
