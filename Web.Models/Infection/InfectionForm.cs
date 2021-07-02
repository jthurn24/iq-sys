using System;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Web.Models.Patient;
using System.Linq;

namespace IQI.Intuition.Web.Models.Infection
{
    public class InfectionForm
    {
        public InfectionForm()
        {
            Patient = new PatientInfo();
            ClientData = new InfectionFormClientData();
        }

        public string CriteriaDefinition { get; set; }

        public Guid Guid { get; set; }

        public string DefaultMD { get; set; }

        public string NursePractitioner { get; set; }

        public int? InfectionVerificationId { get; set; }

        public PatientInfo Patient { get; set; }

        public bool PatientWasHospitalized { get; set; }

        public DateTime? FirstNotedOn { get; set; }

        public bool IsResolved { get; set; }

        public DateTime? ResolvedOn { get; set; }

        public int? InfectionType { get; set; }

        public int? InfectionSite { get; set; }

        public int? InfectionSiteDetail { get; set; }

        public InfectionClassification Classification { get; set; }

        public IEnumerable<int> SelectedCriteria { get; set; }

        public IEnumerable<int> SelectedRiskFactors { get; set; }

        public string AdditionalRiskFactors { get; set; }

        public string Treatement { get; set; }

        public IEnumerable<int> SelectedPrecautions { get; set; }

        public bool LabsPending { get; set; }

        public virtual bool? SpecifiedLabCultureType { get; set; }

        public virtual string LabCultureType { get; set; }

        public virtual bool? SpecifiedLabCbc { get; set; }

        public virtual bool? SpecifiedLabOther { get; set; }

        public virtual string LabOtherDetails { get; set; }

        public virtual string LabResults { get; set; }

        public virtual bool? HadChestXray { get; set; }

        public virtual DateTime? ChestXrayCompletedOn { get; set; }

        public virtual string ChestXRayResults { get; set; }

        public bool IsUpdateMode { get; set; }

        public InfectionFormClientData ClientData { get; set; }

        public IEnumerable<InfectionLabResult> InfectionLabResults { get; set; }

        public IEnumerable<InfectionTreatmentType> InfectionTreatmentTypes { get; set; }

        public IEnumerable<InfectionNote> InfectionNotes { get; set; }

        public int? Floor { get; set; }

        public int? Wing { get; set; }

        public int? Room { get; set; }

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
                    InfectionTypes = ClientData.InfectionTypes,
                    InfectionSites = ClientData.InfectionSites,
                    SelectedInfectionType = InfectionType,
                    SelectedInfectionSite = InfectionSite,
                    SelectedSiteDetail = InfectionSiteDetail,
                    LabTestTypes = ClientData.LabTestTypes,
                    LabResults = InfectionLabResults,
                    TreatmentTypes = InfectionTreatmentTypes,
                    SatisfiedRuleClassifications = ClientData.SatisfiedRuleClassifications,
                    UnSatisfiedRuleClassifications = ClientData.UnSatisfiedRuleClassifications,
                    Classification = System.Enum.GetName(typeof(InfectionClassification),Classification),
                    InfectionNotes = InfectionNotes,
                    Patient = Patient.Guid,
                    InfectionVerificationId = InfectionVerificationId,

                    // It seems an array of ints is not supported by our client side  
                    //  bindings so we convert these values to string arrays
                    SelectedCriteria = SelectedCriteria
                        .EmptyIfNull()
                        .ToStringArray(),
                    SelectedPrecautions = SelectedPrecautions
                        .EmptyIfNull()
                        .ToStringArray()
                };
            }
        }

        public bool CriteriaLocked { get; set; }

        public bool MdNotifiedIneligibleCriteria { get; set; }
    }
}
