using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.ObjectModel.Metadata;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence.Protection;
using IQI.Intuition.Domain.Services.ChangeTracking;

namespace IQI.Intuition.Domain.Models
{
    public class InfectionVerification : AuditTrackingEntity<InfectionVerification>, IRestrictable<Account>, ITrackChanges
    {
        protected InfectionVerification() { }

        public InfectionVerification(Patient patient)
        {
            Patient = patient.ThrowIfNullArgument("patient");

            Guid = GuidHelper.NewGuid();

            // Initialize collections
            _RiskFactors = new List<InfectionRiskFactor>();
            _Criteria = new List<InfectionCriteria>();
            _InfectionNotes = new List<InfectionNote>();
        }

        public virtual bool? LabsPending { get; set; }

        public virtual Guid Guid { get; protected set; }

        public virtual DateTime? FirstNotedOn { get; set; }

        public virtual bool? PatientWasHospitalized { get; set; }

        public virtual bool? SpecifiedLabCultureType { get; set; }

        public virtual string LabCultureType { get; set; }

        public virtual bool? SpecifiedLabCbc { get; set; }

        public virtual bool? SpecifiedLabOther { get; set; }

        public virtual string LabOtherDetailsText { get; set; }

        public virtual string LabResultsText { get; set; }

        public virtual bool? HadChestXray { get; set; }

        public virtual DateTime? ChestXrayCompletedOn { get; set; }

        public virtual string ChestXRayResultsText { get; set; }

        public virtual string MdOrAprnPhysicianName { get; set; }

        public virtual DateTime? ReportEnteredOn { get; set; }

        public virtual string ReportedBy { get; set; }

        public virtual DateTime? LastAdmissionOn { get; set; }

        public virtual bool? IsResolved { get; set; }

        public virtual DateTime? ResolvedOn { get; set; }

        public virtual string TreatementText { get; set; }

        public virtual string AdditionalRiskFactorsText { get; set; }

        public virtual InfectionClassification Classification { get; set; }

        public virtual Patient Patient { get;  set; }

        public virtual InfectionSite InfectionSite { get; set; }

        public virtual bool? Deleted { get; set; }

        public virtual bool? MdNotifiedIneligibleCriteria { get; set; }

        public virtual DateTime? LastSynchronizedAt { get; set; }

        public virtual InfectionSiteSupportingDetail InfectionSiteSupportingDetail { get; set; }

        public virtual IList<InfectionLabResult> LabResults { get; set; }

        public virtual IList<InfectionTreatment> Treatments { get; set; }

        public virtual Room Room { get; set; }

        public virtual string NursePractitioner { get; set; }

        private IList<InfectionRiskFactor> _RiskFactors;
        public virtual IEnumerable<InfectionRiskFactor> RiskFactors
        {
            get
            {
                return _RiskFactors;
            }
        }

        public virtual void AssignRiskFactors(params InfectionRiskFactor[] values)
        {
            // TODO - Should we enforce the rule about having at least one risk factor here?
            UpdateList(_RiskFactors, values);
        }


        private IList<InfectionCriteria> _Criteria;
        public virtual IEnumerable<InfectionCriteria> Criteria
        {
            get
            {
                return _Criteria;
            }
        }

        private IList<InfectionNote> _InfectionNotes;
        public virtual IList<InfectionNote> InfectionNotes
        {
            get
            {
                return _InfectionNotes;
            }
        }

        public virtual void AssignCriteria(params InfectionCriteria[] values)
        {
            if (InfectionSite == null)
            {
                // Since no infection site was indicated there should be no criteria nor can there be any infection
                _Criteria.Clear();
                Classification = InfectionClassification.NoInfection;

                return;
            }

            UpdateList(_Criteria, values);
        }

        private void UpdateList<T>(IList<T> list, T[] values)
        {
            values.ThrowIfNullArgument("values");
            list.Remove(item => !values.Contains(item));
            list.AddRange(values.Except(list));
        }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Patient.Account.Id;
        }

        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<InfectionVerification>()
            .CreateFlag(Enumerations.AuditEntryType.AddedInfection)
            .EditFlag(Enumerations.AuditEntryType.ModifiedInfection)
            .RemoveFlag(Enumerations.AuditEntryType.RemovedInfection)
            .RemoveExpression(x => x.Deleted == true)
            .PatientGuid(x => x.Patient.Guid)
            .ComponentGuid(x => x.Guid)
            .Ignore(x => x.Deleted)
            .Ignore(x => x.LastSynchronizedAt)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.LabResults)
            .Ignore(x => x.Criteria)
            .Ignore(x => x.RiskFactors)
            .Ignore(x => x.Treatments)
            .Ignore(x => x.LastAdmissionOn)
            .Description(x => x.InfectionSite != null ? string.Concat(x.InfectionSite.Type.Name," Infection") : "Infection")
            .OverrideCollection(x => x.Criteria, x => x!= null && x.Count() > 0 ? x.Select(xx => ((InfectionCriteria)xx).Name).ToDelimitedString(',') : string.Empty)
            .OverrideCollection(x => x.RiskFactors, x => x != null && x.Count() > 0 ? x.Select(xx => ((InfectionRiskFactor)xx).Name).ToDelimitedString(',') : string.Empty)
            .GetDefinition();
        }

    }
}
