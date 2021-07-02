using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Web.Models.Patient;

namespace IQI.Intuition.Web.Models.CatheterAssessment
{
    public class AssessmentForm
    {
        public string Id { get; set; }

        public DateTime? AssessmentDate { get; set; }
        public int? ContinuedUseEstimateDays { get; set; }
        public int? Action { get; set; }

        public bool? TubeHolderUsed { get; set; }

        public string TherapyGoal { get; set; }
        public string AlternativeInterventions { get; set; }
        public string CSResults { get; set; }
        public string ReversibleFactors { get; set; }

        public DateTime? NextAssessmentDate { get; set; }


        public string RemovalReason { get; set; }
        public int? RemovalPvr1 { get; set; }
        public int? RemovalPvr1Hours { get; set; }
        public int? RemovalPvr2 { get; set; }
        public int? RemovalPvr2Hours { get; set; }
        public int? RemovalPvr3 { get; set; }
        public int? RemovalPvr3Hours { get; set; }
        public string RemovalComplications { get; set; }
        public string RemoveAlternativeTherapeutic { get; set; }
        public int? RemoveIntakeDaily { get; set; }
        public string RemoveQualityOfUrine { get; set; }
        public string RemoveResidentResponse { get; set; }
        public bool? RemovedAndReplaced { get; set; }
        public string RemovedAndReplacedReason { get; set; }


        public int? Floor { get; set; }
        public int? Wing { get; set; }
        public int? Room { get; set; }

        public AssessmentFormClientData ClientData { get; set; }

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
