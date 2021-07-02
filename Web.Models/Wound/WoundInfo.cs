using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Graphics;

namespace IQI.Intuition.Web.Models.Wound
{
    public class WoundInfo
    {
        public string Id { get; set; }
        public string PatientGuid { get; set; }
        public int PatientId { get; set; }
        public string PatientFullName { get; set; }
        public string RoomWingName { get; set; }
        public string FirstNoted { get; set; }
        public string ResolvedOn { get; set; }
        public string StageName { get; set; }
        public string SiteName { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public string TypeName { get; set; }
        public string AdditionalSiteDetails { get; set; }

        public SeriesLineChart PushChart { get; set; }
    }
}
