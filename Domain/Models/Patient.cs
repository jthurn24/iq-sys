using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Encryption;
using SnyderIS.sCore.Persistence.Protection;
using IQI.Intuition.Domain.Services.ChangeTracking;

namespace IQI.Intuition.Domain.Models
{
    public class Patient : AuditTrackingEntity<Patient>, IRestrictable<Account>, ITrackChanges
    {
        protected Patient() { }

        public Patient(Account account)
        {
            Account = account.ThrowIfNullArgument("account");
            Guid = GuidHelper.NewGuid();

            // Initialize collections
            _Warnings = new List<Warning>();
            _StatusChanges = new List<PatientStatusChange>();
            _RoomChanges = new List<PatientRoomChange>();
            InfectionVerifications = Enumerable.Empty<InfectionVerification>();
            IncidentReports = Enumerable.Empty<IncidentReport>();
            _PatientFlags = new List<PatientFlagType>();
            Catheters = Enumerable.Empty<CatheterEntry>();
            Wounds = Enumerable.Empty<WoundReport>();
            PsychotropicAdministrations = Enumerable.Empty<PsychotropicAdministration>();
        }

        public virtual Guid Guid { get; protected set; }

        public virtual Account Account { get; protected set; }

        public virtual Enumerations.PatientStatus CurrentStatus { get; set; }

        public virtual string GetFirstName()
        {
            if (SecureFirstName.IsNullOrEmpty())
            {
                return ClearFirstName;
            }

            return  GetEncryptionProvider().DecryptString(SecureFirstName);
        }

        public virtual void SetFirstName(string value)
        {
            this.SecureFirstName = GetEncryptionProvider().EncryptString(value);
            this.ClearFirstName = null;
        }

        public virtual string GetMiddleInitial()
        {
            if (SecureMiddleInitial.IsNullOrEmpty())
            {
                return ClearMiddleInitial;
            }

            return  GetEncryptionProvider().DecryptString(SecureMiddleInitial);
        }

        public virtual void SetMiddleInitial(string value)
        {
            if (value.IsNullOrEmpty())
            {
                this.SecureMiddleInitial = null;
            }
            else
            {
                this.SecureMiddleInitial = GetEncryptionProvider().EncryptString(value);
            }

            this.ClearMiddleInitial = null; ;
        }

        public virtual string GetLastName()
        {
            if (SecureLastName.IsNullOrEmpty())
            {
                return ClearLastName;
            }

            return GetEncryptionProvider().DecryptString(SecureLastName);
        }

        public virtual void SetLastName(string value)
        {
            this.SecureLastName = GetEncryptionProvider().EncryptString(value);
            this.ClearLastName = null; ;
        }

        public virtual string ClearFirstName { get; set; }

        public virtual string ClearMiddleInitial { get; set; }

        public virtual string ClearLastName { get; set; }

        public virtual string SecureFirstName { get; set; }

        public virtual string SecureMiddleInitial { get; set; }

        public virtual string SecureLastName { get; set; }

        public virtual int? SecureAlgorithm { get; set; }

        public virtual string MDName { get; set; }

        public virtual DateTime? BirthDate { get; set; }

        public virtual Room Room { get; set; }

        public virtual IEnumerable<InfectionVerification> InfectionVerifications { get; protected set; }

        public virtual IEnumerable<CatheterEntry> Catheters { get; protected set; }

        private IList<PatientFlagType> _PatientFlags;
        public virtual IEnumerable<PatientFlagType> PatientFlags
        {
            get
            {
                return _PatientFlags;
            }
        }

        public virtual IEnumerable<IncidentReport> IncidentReports { get; protected set; }



        public virtual string FullName 
        { 
            get {

                return "{0} , {2} {1}"
                    .FormatWith(GetLastName(), 
                    GetMiddleInitial().WhenNotNullOrWhiteSpace(value => " {0}.".FormatWith(value)), 
                    GetFirstName()).TrimEnd(); 
                
            } 
        }

        public virtual bool? Deleted { get; set; }

        private IList<Warning> _Warnings;
        public virtual IEnumerable<Warning> Warnings
        {
            get
            {
                return _Warnings;
            }
        }

        public virtual IEnumerable<WoundReport> Wounds { get; protected set; }

        public virtual IEnumerable<PsychotropicAdministration> PsychotropicAdministrations { get; protected set; }

        private IList<PatientStatusChange> _StatusChanges;
        public virtual IEnumerable<PatientStatusChange> StatusChanges
        {
            get
            {
                return _StatusChanges;
            }
        }


        private IList<PatientRoomChange> _RoomChanges;
        public virtual IEnumerable<PatientRoomChange> RoomChanges
        {
            get
            {
                return _RoomChanges;
            }
        }

        public virtual DateTime? GetLastAdmissionDate()
        {
            var status = this.StatusChanges
                .Where(x => x.Status == Enumerations.PatientStatus.Admitted)
                .OrderByDescending(x => x.StatusChangedAt)
                .FirstOrDefault();

            if (status == null)
            {
                return null;
            }

            return status.StatusChangedAt;
        }


        public virtual DateTime? GetLastNonAdmissionDate()
        {
            var status = this.StatusChanges
                .Where(x => x.Status != Enumerations.PatientStatus.Admitted)
                .OrderByDescending(x => x.StatusChangedAt)
                .FirstOrDefault();

            if (status == null)
            {
                return null;
            }

            return status.StatusChangedAt;
        }


        public virtual void AddStatusChange(PatientStatusChange change)
        {
            _StatusChanges.Add(change);
        }

        public virtual void AddRoomChange(PatientRoomChange change)
        {
            _RoomChanges.Add(change);
        }

        public virtual void AssignFlags(params PatientFlagType[] values)
        {
            UpdateList(_PatientFlags, values);
        }

        private void UpdateList<T>(IList<T> list, T[] values)
        {
            values.ThrowIfNullArgument("values");
            list.Remove(item => !values.Contains(item));
            list.AddRange(values.Except(list));
        }

        private  IEncryptionHelper GetEncryptionProvider()
        {
            if (this.SecureAlgorithm == Constants.SECURE_ALGORITHM_DES)
            {
                return new DESHelper(Constants.SYSTEM_KEY_DES);
            }

            this.SecureAlgorithm = Constants.SECURE_ALGORITHM_AES;
            return new AESHelper(Constants.SYSTEM_KEY_AES);
        }

        public virtual bool CanBeAccessedBy(Account source)
        {
            return source.Id == this.Account.Id;
        }

        public virtual IChangeTrackingDefinition GetChangeTrackingDefinition()
        {
            return new DefinitionBuilder<Patient>()
            .CreateFlag(Enumerations.AuditEntryType.AddedPatient)
            .EditFlag(Enumerations.AuditEntryType.ModifiedPatientDemographic)
            .RemoveFlag(Enumerations.AuditEntryType.RemovedPatient)
            .RemoveExpression(x => x.Deleted == true)
            .PatientGuid(x => x.Guid)
             .Description(x => "Patient Demographics")
            .Ignore(x => x.Deleted)
            .Ignore(x => x.LastUpdatedBy)
            .Ignore(x => x.LastUpdatedAt)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.CreatedBy)
            .Ignore(x => x.SecureAlgorithm)
            .Ignore(x => x.SecureFirstName)
            .Ignore(x => x.SecureLastName)
            .Ignore(x => x.SecureMiddleInitial)
            .Ignore(x => x.ClearFirstName)
            .Ignore(x => x.ClearLastName)
            .Ignore(x => x.ClearMiddleInitial)
            .Ignore(x => x.ClearFirstName)
            .Ignore(x => x.StatusChanges)
            .OverrideCollection(x => x.PatientFlags, x => x != null && x.Count() > 0 ? x.Select(xx => ((PatientFlagType)xx).Name).ToDelimitedString(',') : string.Empty)
            .GetDefinition();
        }
    }
}
