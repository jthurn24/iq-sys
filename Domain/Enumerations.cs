using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Domain
{
    public class Enumerations
    {
        public enum KnownPermision
        {
            ManageUsers = 1,
            EditResolvedInfections = 2,
            RemoveInfections = 3,
            RemovePatients = 4,
            ViewWarningRules = 5,
            ViewAndEditEmployeeInfections = 6,
            EditResolvedIncidents = 7,
            RemoveIncidents = 8,
            ViewAndEditComplaints = 9,
            ManageCmsRecords = 10,
            RemovePsychotropic = 13,
            RemoveWounds = 14,
            RemoveCatheter = 15,
            ManageQICast = 16
        }

        public enum AuditEntryType
        {
            FailedLogin = 1,
            SuccessfulLogin = 2,
            
            RemovedPatient = 3,
            AddedPatient = 10,
            ModifiedPatientDemographic = 11,

            AddedEmployeeInfection = 14,
            RemovedEmployeeInfection = 5,
            ModifiedEmployeeInfection = 15,

            RemovedComplaint = 6,
            AddedComplaint = 12,
            ModifiedComplaint = 13,
            
            AddedIncident = 17,
            RemovedIncident = 7,
            ModifiedIncident = 9,

            AddedInfection = 16,
            ModifiedInfection = 8,
            RemovedInfection = 4,

            AddedWound = 18,
            ModifiedWound = 19,
            RemovedWound = 20,


            AddedPsychotropic = 21,
            ModifiedPsychotropic = 22,
            RemovedPsychotropic = 23,

            Unknown = 24,

            AddedCatheter = 25,
            ModifiedCatheter = 26,
            RemovedCatheter = 27

        }

        public enum PatientStatus
        {
            Admitted = 1,
            Discharged = 2,
            Expired = 3
        }

        public enum EmployeeShift
        {
            First = 1,
            Second = 2,
            Third = 3
        }

        public enum EmployeeDepartment
        {
            Admissions,
            Administrations,
            BillingAndOfficeStaff,
            CertifiedNursingAssistant,
            Dietary,
            EducationManagementStaff,
            Housekeeping,
            Laundry,
            Maintainance,
            Nursing,
            Other,
            Security,
            SocialServices,
            UnitOrWingStaff,
            Volunteer
        }

        public enum Gender
        {
            Unknown = 1,
            Male = 2,
            Female = 3
        }

        public enum KnownProductType
        {
            InfectionTracking = 1,
            IncidentTracking = 2,
            ComplaintTracking = 3,
            CmsCompliance = 4,
            PsychotropicTracking =5,
            WoundTracking = 6,
            VaccineTracking = 7
        }

        public enum ProductFeeType
        {
            MonthlyPatientCensus = 1,
            OneTime = 2,
            Yearly = 3
        }

        public enum InjuryLevel
        {
            NoInjury = 1,
            MinorInjury = 2,
            MajorInjury = 3,
            Death = 4
        }

        public enum SystemTicketStatus
        {
            New = 1,
            Closed = 2,
            InProgress = 3,
            Test = 4
        }

        public enum SystemTicketSearchMode
        {
            OpenOnly,
            ClosedOnly,
            All
        }

        public enum KnownSystemTicketType
        {
            SupportRequest = 1,
            FeatureRequest = 2,
            ActionItem = 3
        }

        public enum SystemLeadMailingStatus
        {
            NotAvailable = 1,
            Mail = 2,
            Set = 3
        }



        public enum SystemLeadStatus
        {
            Call = 10,
            MailingListSignUp = 20,
            VoiceMail = 30,
            DemoScheduled = 40,
            DemoComplete = 50,
            NotInterested = 60,
            Warm = 70,
            Hot = 80,
            Converted = 90
        }

        public enum FacilityType
        {
            SkilledNursing = 1,
            AssistedLiving = 2,
            Hospital = 3,
            MultiComplex = 4
        }

        public enum InfectionCriteriaAvailabilityRule
        {
            None = 1,
            Cdiff = 2
        }

        public enum InfectionMetric
        {
            Rate,
            NosocomialTotal
        }

        public enum KnownCmsMatrixType
        {
            RosterCMS802 = 1
        }

        public enum SystemUserRole
        {
            Admin = 1,
            Prospector = 2
        }

        public enum ResourceType
        {
            Link = 0,
            File = 1
        }

        public enum WoundClassification
        {
            FacilityAquired = 0,
            AdmittedFromHospice = 1,
            AdmittedFromNursingHome = 2,
            AdmittedFromHospital = 3,
            AdmittedFromHomeHealth = 4,
            AdmittedFromHome = 5
        }

        public enum DrugType
        {
            Prescription = 0
        }


        public enum WoundExudate
        {
            None = 0,
            Scant = 1,
            Moderate = 2,
            Heavy = 3,
            Sanguineous = 4
        }

        public enum WoundExudateType
        {
            None = 0,
            Serous = 1,
            SerosanGuinous = 2,
            Tan = 3,
            Brown = 4,
            Green = 5,
            Yellow = 6
        }


        public enum WoundEdge
        {
            None = 0,
            Healthy = 1,
            Pink = 2,
            Red = 3,
            Purple = 4,
            Black = 5,
            Maceration = 6,
            Induration = 7,
            Attached = 8,
            Detached = 9,
            RolledUnder = 10,
            Calloused = 11,
            Dry = 12,
            Other = 13
        }

        public enum WoundPeriwoundTissue
        {
            None = 0,
            Healthy = 1,
            Maceration = 2,
            Indurations = 3,
            PeripheralTissueEdema = 4,
            Excoriated = 5,
            Erythema = 6,
            Periwoundwarmth = 7
        }

        public enum WoundProgress
        {
            NoChange = 0,
            Improved = 1,
            Infected = 2,
            Deteriorated = 3,
            Closed = 4,
            New = 5
        }


        public enum WoundTreatmentStatus
        {
            ContinueTreatment = 0,
            ChangeTreatment = 1
        }

        public enum KnownWoundType
        {
            PressureUlcer = 1
        }

        public enum KnownWoundStage
        {
            NotApplicable = 7,
            Stage1 = 1,
            Stage2 = 2,
            Stage3 = 3,
            Stage4 = 4
        }

        public enum VaccineStatus
        {
            NeverActive = 0,
            Inactive = 1,
            Active = 2,
            Pending = 3
        }

        public enum CatheterType
        {
            Urethral = 1,
            SupraPubic,
            Condom
        }

        public enum CatheterReason
        {
            Diuretic = 1,
            Retention,
            Ulcer,
            PostSurgery,
            Other
        }

        public enum CatheterMaterial
        {
            Latex = 1,
            Silicone,
            Other
        }

        public enum CatheterAction
        {
            Continue = 1,
            Attempt
        }

        public enum NotificationMethod
        {
            Email = 1,
            CellPhone =2,
            CellPhoneAndEmail =3
        }
    }
}
