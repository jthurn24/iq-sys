using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Encryption;


namespace IQI.Intuition.Domain.Models
{
    public class EmployeeInfection : AuditTrackingEntity<EmployeeInfection>
    {
        protected EmployeeInfection() { }

        public EmployeeInfection(Facility facility)
        {
            Facility = facility.ThrowIfNullArgument("facility");
            Guid = GuidHelper.NewGuid();
            _InfectionSymptoms = new List<InfectionSymptom>();
        }

        public virtual DateTime? NotifiedOn {get; set;}
        
        public virtual string SecureFirstName { get; set; }
             
        public virtual string SecureLastName { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual Enumerations.Gender Gender { get; set; }

        public virtual Enumerations.EmployeeShift LastShift { get; set; }

        public virtual DateTime? LastWorkedOn { get; set; }

        public virtual Enumerations.EmployeeDepartment Department { get; set; }

        public virtual Wing Wing { get; set; }

        public virtual Facility Facility { get; set; }

        public virtual string Notes { get; set; }

        public virtual DateTime? FirstSymptomOn { get; set; }

        public virtual InfectionType InfectionType { get; set; }

        public virtual bool? SeenByMD { get; set; }

        public virtual string MDInstructions { get; set; }

        public virtual string LabResults { get; set; }

        public virtual DateTime? LastSymptomOn { get; set; }

        public virtual DateTime? WellOn { get; set; }

        public virtual Guid Guid { get; set; }

        public virtual bool? Deleted { get; set; }

        public virtual DateTime? ReturnToWorkOn { get; set; }

        public virtual int? SecureAlgorithm { get; set; }

        public virtual string GetFirstName()
        {
            if (SecureFirstName.IsNullOrEmpty())
            {
                return string.Empty;
            }

            return GetEncryptionProvider().DecryptString(SecureFirstName);
        }

        public virtual void SetFirstName(string value)
        {
            this.SecureFirstName = GetEncryptionProvider().EncryptString(value);
        }

        public virtual string GetLastName()
        {
            if (SecureLastName.IsNullOrEmpty())
            {
                return string.Empty;
            }

            return  GetEncryptionProvider().DecryptString(SecureLastName);
        }

        public virtual void SetLastName(string value)
        {
            this.SecureLastName = GetEncryptionProvider().EncryptString(value);
        }

        public virtual string FullName
        {
            get
            {

                return "{0} , {1}"
                    .FormatWith(GetLastName(),
                    GetFirstName()).TrimEnd();

            }
        }

        private IList<InfectionSymptom> _InfectionSymptoms;
        public virtual IEnumerable<InfectionSymptom> InfectionSymptoms
        {
            get
            {
                return _InfectionSymptoms;
            }
        }

        public virtual void AssignSymptoms(params InfectionSymptom[] values)
        {
            UpdateList(_InfectionSymptoms, values);
        }

        private void UpdateList<T>(IList<T> list, T[] values)
        {
            values.ThrowIfNullArgument("values");
            list.Remove(item => !values.Contains(item));
            list.AddRange(values.Except(list));
        }

        private IEncryptionHelper GetEncryptionProvider()
        {
            if (this.SecureAlgorithm == Constants.SECURE_ALGORITHM_DES)
            {
                return new DESHelper(Constants.SYSTEM_KEY_DES);
            }

            this.SecureAlgorithm = Constants.SECURE_ALGORITHM_AES;
            return new AESHelper(Constants.SYSTEM_KEY_AES);
        }
    }
}
