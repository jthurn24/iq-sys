using System;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Web.Models.Patient;
using System.Web.Mvc;

namespace IQI.Intuition.Web.Models.Incident
{
    public class IncidentForm
    {
        public IncidentForm()
        {
            Patient = new PatientInfo();
            ClientData = new IncidentFormClientData();
        }

        public int? IncidentReportId { get; set; }
        public Guid Guid { get; set; }
        public PatientInfo Patient { get; set; }
        public IEnumerable<int> SelectedIncidentTypes { get; set; }
        public IEnumerable<int> SelectedIncidentInjuries { get; set; }
        public DateTime? DiscoveredOn { get; set; }
        public int DiscoveredHour { get; set; }
        public int DiscoveredMinutes { get; set; }

        public int? FoundById { get; set; }
        public string FoundByName { get; set; }

        public int? CNAId { get; set; }
        public string CNAName { get; set; }

        public DateTime? OccurredOn { get; set; }
        public int OccurredHour { get; set; }
        public int OccurredMinutes { get; set; }
        public bool OcurredUnknown { get; set; }

        public string ResidentStatement { get; set; }
        public string InjuryAndTreatmentDescription { get; set; }
        public string Temperature { get; set; }
        public string Pulse { get; set; }
        public string Respiratory { get; set; }
        public string BloodPressureStanding { get; set; }
        public string BloodPressureSitting { get; set; }
        public bool? NeuroCheckCompleted { get; set; }
        public string BloodGlucos { get; set; }
        public int? IncidentLocation { get; set; }
        public string LocationDetails { get; set; }
        public bool? AssessmentCompleted { get; set; }
        public Domain.Enumerations.InjuryLevel? InjuryLevel { get; set; }

        public bool IsUpdateMode { get; set; }

        public IEnumerable<IncidentWitness> IncidentWitnesses { get; set; }

        public int? Floor { get; set; }

        public int? Wing { get; set; }

        public int? Room { get; set; }
        
        public IncidentFormClientData ClientData { get; set; }

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
                    Rooms = ClientData.Rooms,
                    Witnesses = IncidentWitnesses,
                    OcurredUnknown = OcurredUnknown
                };
            }
        }
      
    }
}
