using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Extensions.IO;

namespace IQI.Intuition.Web.Models.Reporting.Infection.Facility
{
    public class LineListingInfectionView
    {

        public IEnumerable<SelectListItem> WingOptions { get; set; }
        public int? Wing { get; set; }

        public IEnumerable<SelectListItem> FloorOptions { get; set; }
        public int? Floor { get; set; }

        public bool IncludeResolved { get; set; }
        public int DisplayMode { get; set; }
        public IEnumerable<SelectListItem> DisplayModeOptions { get; set; }
        
        public int? InfectionType { get; set; }
        public IEnumerable<SelectListItem> InfectionTypeOptions { get; set; }

        public IList<int> Pathogens { get; set; }
        public IEnumerable<SelectListItem> PathogenOptions { get; set; }

        public IList<string> Antibiotics { get; set; }
        public IEnumerable<SelectListItem> AntibioticOptions { get; set; }

        public IList<int> LabTests { get; set; }
        public IEnumerable<SelectListItem> LabTestOptions { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IncludePendingLabsOnly { get; set; }

        public IList<InfectionRow> Infections { get; set; }

        public void SetData(IEnumerable<InfectionVerification> infectionData, IEnumerable<PatientPrecaution> precautionData)
        {
            this.Infections = new List<InfectionRow>();

            foreach(var infection in infectionData)
            {
                var patientPrecautions = precautionData.Where(x => x.Patient.Id == infection.Patient.Id);
                var row = new InfectionRow(infection, patientPrecautions);
                this.Infections.Add(row);
            }
        }

        public class InfectionRow
        {
            public string PatientName { get; set; }
            public string Date { get; set; }
            public string Room { get; set; }
            public string Wing { get; set; }
            public string Floor { get; set; }
            public string AdmissionDate { get; set; }

            public string NursePractitioner { get; set; }

            public IEnumerable<string> Reasons { get; set; }
            public IEnumerable<string> RiskFactors { get; set; }
            public IEnumerable<string> Flags { get; set; }
            public string ChestXrayDate { get; set; }
            public string ChestXrayResults { get; set; }
            public bool HasChestXrayResults { get; set; }
            public string Therapy { get; set; }
            public IEnumerable<string> Precautions { get; set; }
            public IList<LineListingLabResultView> LabResults { get; set; }
            public IList<LineListingTreatmentView> Treatments { get; set; }
            public string PatientBirthDate { get; set; }
            public string ResolvedDate { get; set; }
            public string Classification { get; set; }
            public string InfectionType { get; set; }
            public string InfectionTypeDetails { get; set; }

            public string AdditionalRiskFactors { get; set; }
            public string ID { get; set; }
            public IList<LineListingNoteView> Notes { get; set; }

            public string RecentInfectionsDescription { get; set; }
            public IEnumerable<string> RecentInfectionDates { get; set; }

            public bool MdNotifiedIneligibleCriteria { get; set; }

            public bool LabsPending { get; set; }

            public Guid PatientGuid { get; set; }
            public Guid InfectionGuid { get; set; }

            public InfectionRow(InfectionVerification infection, IEnumerable<PatientPrecaution> precautions)
            {
                PatientName = infection.Patient.FullName;
                Date = infection.FirstNotedOn.FormatAs("MM/dd/yyyy");
                Therapy = infection.TreatementText;
                Reasons = infection.Criteria.Select(x => x.Name);
                Room = infection.Room.Name;
                Wing = infection.Room.Wing.Name;
                Floor = infection.Room.Wing.Floor.Name;
                ResolvedDate = infection.ResolvedOn.FormatAsShortDate();
                Classification = System.Enum.GetName(typeof(InfectionClassification), infection.Classification).SplitPascalCase();
                PatientBirthDate = infection.Patient.BirthDate.FormatAsShortDate();
                InfectionType = infection.InfectionSite.Type.Name;
                ID = infection.Id.ToString();
                ChestXrayDate = infection.ChestXrayCompletedOn.HasValue == true ? infection.ChestXrayCompletedOn.Value.ToString("MM/dd/yy") : string.Empty;
                ChestXrayResults = infection.ChestXRayResultsText;
                HasChestXrayResults = infection.HadChestXray.GetValueOrDefault(false);
                MdNotifiedIneligibleCriteria = infection.MdNotifiedIneligibleCriteria == true ? true : false;
                InfectionGuid = infection.Guid;
                PatientGuid = infection.Patient.Guid;
                LabsPending = infection.LabsPending.HasValue ? infection.LabsPending.Value : false;
                NursePractitioner = infection.NursePractitioner;

                var lastAdmitDate = infection.Patient.GetLastAdmissionDate();

                if (lastAdmitDate == null)
                {
                    AdmissionDate = string.Empty;
                }
                else
                {
                    AdmissionDate = lastAdmitDate.Value.ToString("MM/dd/yy");
                }

                var infectionTypeDetailsBuilder = new System.Text.StringBuilder();

                infectionTypeDetailsBuilder.Append(infection.InfectionSite.Name);

                if (infection.InfectionSiteSupportingDetail != null)
                {
                    infectionTypeDetailsBuilder.Append(" ( ");
                    infectionTypeDetailsBuilder.Append(infection.InfectionSite.SupportingDetailsDescription);
                    infectionTypeDetailsBuilder.Append(" ");
                    infectionTypeDetailsBuilder.Append(infection.InfectionSiteSupportingDetail.Name);
                    infectionTypeDetailsBuilder.Append(" ) ");
                }

                InfectionTypeDetails = infectionTypeDetailsBuilder.ToString();


                Treatments = new List<LineListingTreatmentView>();

                foreach (var treatment in infection.Treatments.OrderBy(x => x.CreatedAt))
                {
                    var t = new LineListingTreatmentView();
                    t.DeliveryMethod = treatment.DeliveryMethod;
                    t.Dosage = treatment.Dosage;
                    t.SpecialInstructions = treatment.SpecialInstructions;
                    t.TreatmentName = treatment.TreatmentName;
                    t.Units = treatment.Units;
                    t.Frequency = treatment.Frequency;
                    t.MDName = treatment.MDName;
                    t.Duration = treatment.Duration;

                    if (treatment.AdministeredOn.HasValue)
                    {
                        t.AdministeredOn = treatment.AdministeredOn.Value.ToString("MM/dd/yy");
                    }
                    else
                    {
                        t.AdministeredOn = string.Empty;
                    }

                    Treatments.Add(t);

                }

                Notes = new List<LineListingNoteView>();

                foreach (var note in infection.InfectionNotes.OrderBy(x => x.CreatedAt))
                {
                    var n = new LineListingNoteView();
                    n.CreatedOn = note.CreatedAt.Value.ToString("MM/dd/yy");
                    n.Note = note.Note;
                    Notes.Add(n);
                }

                LabResults = new List<LineListingLabResultView>();

                foreach (var result in infection.LabResults)
                {
                    var r = new LineListingLabResultView();

                    if (result.CompletedOn.HasValue)
                    {
                        r.CompletedOn = result.CompletedOn.Value.ToString("MM/dd/yyyy");
                    }
                    else
                    {
                        r.CompletedOn = string.Empty;
                    }

                    r.Notes = result.Notes;
                    r.Result = result.LabResult.Name;
                    r.TestType = result.LabTestType.Name;
                    r.Pathogens = new List<string>();

                    foreach (var p in result.ResultPathogens)
                    {
                        if (p.PathogenQuantity != null)
                        {
                            r.Pathogens.Add(string.Concat(p.Pathogen.Name, " ", p.PathogenQuantity.Name));
                        }
                        else
                        {
                            r.Pathogens.Add(p.Pathogen.Name);
                        }
                    }

                    LabResults.Add(r);
                }

                RiskFactors = infection.RiskFactors.Select(x => x.Name);
                AdditionalRiskFactors = infection.AdditionalRiskFactorsText;
                Flags = infection.Patient.PatientFlags.Select(x => x.Name);

                RecentInfectionsDescription = string.Format("Other confirmed {0} infections:", infection.InfectionSite.Type.Name);
                RecentInfectionDates = infection.Patient.InfectionVerifications
                    .Where(x => x.InfectionSite.Type == infection.InfectionSite.Type)
                    .Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.HealthCareAssociatedInfection || x.Classification == InfectionClassification.AdmissionHospitalDiagnosed)
                    .Where(x => x.Deleted == null || x.Deleted == false)
                    .Where(x => x.Id != infection.Id)
                    .OrderBy(x => x.FirstNotedOn)
                    .Select(x => x.FirstNotedOn.FormatAsShortDate());


                var relatedPrecautions = new List<string>();

                foreach(var prec in precautions)
                {
                    var endInfection = infection.ResolvedOn.HasValue ? infection.ResolvedOn.Value : DateTime.Today;
                    var endPrecuation = prec.EndDate.HasValue ? prec.EndDate.Value : DateTime.Today;

                    if (prec.StartDate <= endInfection && endPrecuation >= infection.FirstNotedOn)
                    {
                        relatedPrecautions.Add(prec.PrecautionType.Name);
                    }

                }

                Precautions = relatedPrecautions;

            }
        }
    }
}
