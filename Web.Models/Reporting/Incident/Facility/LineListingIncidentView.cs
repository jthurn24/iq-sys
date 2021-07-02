using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;

namespace IQI.Intuition.Web.Models.Reporting.Incident.Facility
{
    public class LineListingIncidentView
    {
        public IEnumerable<SelectListItem> IncidentGroupOptions { get; set; }
        public List<string> SelectedIncidentGroups { get; set; }

        public IEnumerable<SelectListItem> InjuryTypeOptions { get; set; }
        public List<int> SelectedInjuryTypes { get; set; }

        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? Wing { get; set; }

        public IEnumerable<SelectListItem> FloorOptions { get; set; }
        public int? Floor { get; set; }

        public IList<IncidentRow> Incidents { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public void SetData(IEnumerable<IncidentReport> incidentData, IEnumerable<PatientPrecaution> precautionData)
        {
            this.Incidents = new List<IncidentRow>();

            foreach (var incident in incidentData)
            {
                var patientPrecautions = precautionData.Where(x => x.Patient.Id == incident.Patient.Id);
                var row = new IncidentRow(incident, patientPrecautions);
                this.Incidents.Add(row);
            }
        }

        public class IncidentRow
        {
            public string PatientName { get; set; }
            public string Date { get; set; }
            public string Room { get; set; }
            public string Wing { get; set; }
            public string Floor { get; set; }
            public string AdmissionDate { get; set; }
            public string ID { get; set; }
            public List<string> IncidentTypes { get; set; }
            public string PatientBirthDate { get; set; }
            public string DiscoveredOn { get; set; }
            public string OccurredOn { get; set; }
            public string ResidentStatement { get; set; }
            public string InjuryAndTreatmentDescription { get; set; }
            public List<string> IncidentInjuries { get; set; }
            public string Temperature { get; set; }
            public string Pulse { get; set; }
            public string Respiratory { get; set; }
            public string BloodPressureStanding { get; set; }
            public string BloodPressureSitting { get; set; }
            public bool NeuroCheckCompleted { get; set; }
            public string BloodGlucos { get; set; }
            public string IncidentLocation { get; set; }
            public string LocationDetails { get; set; }
            public string AssessmentCompleted { get; set; }
            public string InjuryLevel { get; set; }
            public string FoundBy { get; set; }
            public string CNA { get; set; }
            public IEnumerable<string> Precautions { get; set; }

            public IList<LineListingWitnessView> Witnesses { get; set; }

            public IEnumerable<IncidentWitness> IncidentWitnesses { get; set; }

            public IncidentRow(IncidentReport incident, IEnumerable<PatientPrecaution> precautions)
            {
                PatientName = incident.Patient.FullName;
                Room = incident.Patient.Room.Name;
                Wing = incident.Patient.Room.Wing.Name;
                Floor = incident.Patient.Room.Wing.Floor.Name;
                PatientBirthDate = incident.Patient.BirthDate.FormatAs("MM/dd/yyyy");
                ID = incident.Id.ToString();
                DiscoveredOn = incident.DiscoveredOn.FormatAs("MM/dd/yyyy HH:mm");
                OccurredOn = incident.OccurredOn.FormatAs("MM/dd/yyyy HH:mm");
                ResidentStatement = incident.ResidentStatement;
                InjuryAndTreatmentDescription = incident.InjuryAndTreatmentDescription;
                Temperature = incident.Temperature;
                Pulse = incident.Pulse;
                Respiratory = incident.Respiratory;
                BloodPressureStanding = incident.BloodPressureStanding;
                BloodPressureSitting = incident.BloodPressureSitting;
                NeuroCheckCompleted = incident.NeuroCheckCompleted.HasValue ? incident.NeuroCheckCompleted.Value : false;
                BloodGlucos = incident.BloodGlucos;
                IncidentLocation = incident.IncidentLocation.Name;
                LocationDetails = incident.LocationDetails;
                AssessmentCompleted = incident.AssessmentCompleted.FormatAsAnswer();
                InjuryLevel = System.Enum.GetName(typeof(Domain.Enumerations.InjuryLevel), incident.InjuryLevel).SplitPascalCase();

                FoundBy = incident.Employee != null ? incident.Employee.FullName : string.Empty;
                CNA = incident.Employee2 != null ? incident.Employee2.FullName : string.Empty;

                var lastAdmitDate = incident.Patient.GetLastAdmissionDate();

                if (lastAdmitDate == null)
                {
                    AdmissionDate = string.Empty;
                }
                else
                {
                    AdmissionDate = lastAdmitDate.Value.ToString("MM/dd/yy");
                }

                IncidentTypes = incident.IncidentTypes.Select(x => x.Name).ToList();

                IncidentInjuries = incident.IncidentInjuries.Select(x => x.Name).ToList();

                Witnesses = incident.IncidentWitnesses.Select(x => new LineListingWitnessView()
                {
                     Name = x.Name,
                     Role = x.Role,
                     Statement = x.Statement
                }).ToList();

                var relatedPrecautions = new List<string>();

                foreach (var prec in precautions)
                {
                    if (incident.IncidentTypes.Count() > 0)
                    {
                        if (prec.PrecautionType.SubProductTypeKey == incident.IncidentTypes.First().GroupName)
                        {
                            var targetDate = incident.OccurredOn.HasValue ? incident.OccurredOn.Value : incident.DiscoveredOn.Value;

                            if (prec.StartDate <= targetDate)
                            {
                                if (prec.EndDate.HasValue == false || prec.EndDate >= targetDate)
                                {
                                    relatedPrecautions.Add(prec.PrecautionType.Name);
                                }
                            }
                        }
                    }
                }

                Precautions = relatedPrecautions;

            }
        }
    }
}
