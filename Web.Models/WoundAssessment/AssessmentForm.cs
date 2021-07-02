using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.Patient;

namespace IQI.Intuition.Web.Models.WoundAssessment
{
    public class AssessmentForm
    {

        public string Id { get; set; }
        public int? Stage { get; set; }
        public bool StageLocked { get; set; }
        
        public DateTime? AssessmentDate { get; set; }
        public int? Floor { get; set; }
        public int? Wing { get; set; }
        public int? Room { get; set; }

        public AssessmentFormClientData ClientData { get; set; }


        public decimal? Lcm { get; set; }
        public decimal? Wcm { get; set; }
        public decimal? Dcm { get; set; }
        public bool? Superficial { get; set; }
        public int? Undermining1From { get; set; }
        public int? Undermining1To { get; set; }
        public decimal? Undermining1Depth { get; set; }
        public int? Undermining2From { get; set; }
        public int? Undermining2To { get; set; }
        public decimal? Undermining2Depth { get; set; }
        public int? Undermining3From { get; set; }
        public int? Undermining3To { get; set; }
        public decimal? Undermining3Depth { get; set; }
        public int? Tunnel1Location { get; set; }
        public decimal? Tunnel1Depth { get; set; }
        public int? Tunnel2Location { get; set; }
        public decimal? Tunnel2Depth { get; set; }
        public int? Tunnel3Location { get; set; }
        public decimal? Tunnel3Depth { get; set; }
        public Domain.Enumerations.WoundExudate Exudate { get; set; }
        public Domain.Enumerations.WoundExudateType ExudateType { get; set; }
        public bool? Odor { get; set; }
        public Decimal? WoundBedEpithelial { get; set; }
        public Decimal? WoundBedGranulation { get; set; }
        public Decimal? WoundBedSlough { get; set; }
        public Decimal? WoundBedNecrosis { get; set; }
        public string WoundBedOther { get; set; }
        public Domain.Enumerations.WoundEdge WoundEdge { get; set; }
        public Domain.Enumerations.WoundPeriwoundTissue PeriwoundTissue { get; set; }
        public bool? Pain { get; set; }
        public string PainManagedWith { get; set; }
        public Domain.Enumerations.WoundProgress Progress { get; set; }
        public int? PushScore { get; set; }
        public Domain.Enumerations.WoundTreatmentStatus TreatmentStatus { get; set; }


        public object ClientViewModel
        {
            get
            {
                return new
                {
                    Floor = Floor,
                    Wing = Wing,
                    Room = Room,
                    Floors = ClientData.Floors,
                    Wings = ClientData.Wings,
                    Rooms = ClientData.Rooms
                };
            }
        }
    }
}
