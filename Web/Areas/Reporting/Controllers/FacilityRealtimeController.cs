using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.Mvc.Extensions;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Web.Models.Reporting;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Extensions;

namespace IQI.Intuition.Web.Areas.Reporting.Controllers
{
    [SupportsTokenAuthentication, EnableAccountRestriction]
    public class FacilityRealtimeController : Controller
    {
        private Facility _Facility;

        public FacilityRealtimeController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            IInfectionRepository infectionRepository,
            IEmployeeInfectionRepository employeeInfectionRepository,
            IIncidentRepository incidentRepository,
            IPatientRepository patientRepository,
            IComplaintRepository complaintRepository,
            IEmployeeRepository employeeRepository,
            ILabResultRepository labResultRepository,
            ITreatmentRepository treatmentRepository,
            ICmsMatrixRepository cmsMatrixRepository,
            IPsychotropicRespository psychotropicRespository,
            IWoundRepository woundRepository,
            ISystemRepository systemRepository,
            ICatheterRepository catheterRepository,
            IVaccineRepository vaccineRepository,
            IPrecautionRepository precautionRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository"); 
            EmployeeInfectionRepository = employeeInfectionRepository.ThrowIfNullArgument("employeeInfectionRepository");
            IncidentRepository = incidentRepository.ThrowIfNullArgument("incidentRepository");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            ComplaintRepository = complaintRepository.ThrowIfNullArgument("complaintRepository");
            EmployeeRepository = employeeRepository.ThrowIfNullArgument("employeeRepository");
            LabResultRepository = labResultRepository.ThrowIfNullArgument("labResultRepository");
            TreatmentRepository = treatmentRepository.ThrowIfNullArgument("treatmentRepository");
            CmsMatrixRepository = cmsMatrixRepository.ThrowIfNullArgument("cmsMatrixRepository");
            PsychotropicRespository = psychotropicRespository.ThrowIfNullArgument("psychotropicRespository");
            WoundRepository = woundRepository.ThrowIfNullArgument("WoundRepository");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
            CatheterRepository = catheterRepository.ThrowIfNullArgument("catheterRepository");
            VaccineRepository = vaccineRepository.ThrowIfNullArgument("vaccineRepository");
            PrecautionRepository = precautionRepository.ThrowIfNullArgument("precautionRepository");

            _Facility = ActionContext.CurrentFacility;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IInfectionRepository InfectionRepository { get; private set; }
        protected virtual IEmployeeInfectionRepository EmployeeInfectionRepository { get; private set; }
        protected virtual IIncidentRepository IncidentRepository { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual IComplaintRepository ComplaintRepository { get; private set; }
        protected virtual IEmployeeRepository EmployeeRepository { get; private set; }
        protected virtual ILabResultRepository LabResultRepository { get; private set; }
        protected virtual ITreatmentRepository TreatmentRepository { get; private set; }
        protected virtual ICmsMatrixRepository CmsMatrixRepository { get; private set; }
        protected virtual IPsychotropicRespository PsychotropicRespository { get; private set; }
        protected virtual IWoundRepository WoundRepository { get; private set; }
        protected virtual ISystemRepository SystemRepository  { get; private set; }
        protected virtual ICatheterRepository CatheterRepository { get; private set; }
        protected virtual IVaccineRepository VaccineRepository { get; private set; }
        protected virtual IPrecautionRepository PrecautionRepository { get; private set; }

        public ActionResult LineListingEmployeeInfection(Models.Reporting.EmployeeInfection.Facility.LineListingInfectionView model)
        {
            if (model.StartDate.HasValue == false)
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1,0,0,0);
            }

            if (model.EndDate.HasValue == false)
            {
                model.EndDate = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
            }

            var infectionTypes = new List<SelectListItem>();
            infectionTypes.Add(new SelectListItem() { Text = "Show All", Value = string.Empty });

            foreach (var infectionType in InfectionRepository.AllInfectionTypes.Where(x => x.UsedForEmployees == true))
            {
                infectionTypes.Add(new SelectListItem() { Text = infectionType.Name, Value = infectionType.Id.ToString() });
            }

            infectionTypes.Add(new SelectListItem() { Text = "Other (Non-Infection)", Value = "-1" });

            model.InfectionTypeOptions = infectionTypes;

            var infections = this.EmployeeInfectionRepository.FindForLineListing(
                ActionContext.CurrentFacility,
                model.StartDate,
                model.EndDate,
                model.InfectionType < 0 ? null : model.InfectionType );

            if (model.InfectionType < 0)
            {
                infections = infections.Where(x => x.InfectionType == null);
            }

            model.SetData(infections);

            return View(model);
        }

        public ActionResult LineListingInfection(Models.Reporting.Infection.Facility.LineListingInfectionView model)
        {
            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name," - ",m.Name) }).Prepend(new SelectListItem() { Text = "Show All", Value= string.Empty});
            model.FloorOptions = _Facility.Floors.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });
            model.PathogenOptions = LabResultRepository.AllPathogens.OrderBy(x => x.Name).Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });
            model.AntibioticOptions = TreatmentRepository.AllTreatmentTypes.SelectMany(x => x.Treatments).ToList().OrderBy(x => x.Name).Select(m => new SelectListItem() { Value = m.Name, Text = m.Name });
            model.LabTestOptions = LabResultRepository.AllTestTypes.OrderBy(x => x.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });


            if (model.Wing < 1)
            {
                model.Wing = wings.FirstOrDefault().Id;
            }

            var displayModes = new List<SelectListItem>();
            displayModes.Add(new SelectListItem() { Text = "Show All", Value = "0" });
            displayModes.Add(new SelectListItem() { Text = "Show Confirmed Only", Value = "1" });
            displayModes.Add(new SelectListItem() { Text = "Show HAI Only", Value = "2" });
            displayModes.Add(new SelectListItem() { Text = "Show Admission Only", Value = "3" });
            model.DisplayModeOptions = displayModes;

            var infectionTypes = new List<SelectListItem>();
            infectionTypes.Add(new SelectListItem() { Text = "Show All", Value = string.Empty });

            foreach (var infectionType in InfectionRepository.AllInfectionTypes.Where(x => x.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id))
            {
                infectionTypes.Add(new SelectListItem() { Text = infectionType.Name, Value = infectionType.Id.ToString() });
            }


            model.InfectionTypeOptions = infectionTypes;

            var infections = this.InfectionRepository.FindForLineListing(
                ActionContext.CurrentFacility,
                model.Wing,
                model.Floor,
                model.IncludeResolved,
                model.StartDate,
                model.EndDate,
                model.InfectionType,
                model.IncludePendingLabsOnly,
                model.Pathogens,
                model.Antibiotics,
                model.LabTests);

            if (model.DisplayMode == 1)
            {
                infections = infections.Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.HealthCareAssociatedInfection).ToList();
            }
            else if (model.DisplayMode == 2)
            {
                infections = infections.Where(x => x.Classification == InfectionClassification.HealthCareAssociatedInfection).ToList();
            }
            else if (model.DisplayMode == 3)
            {
                infections = infections.Where(x => x.Classification == InfectionClassification.Admission).ToList();
            }


            var precautionData = PrecautionRepository.GetForFacilityAndProduct(ActionContext.CurrentFacility.Id, 
                Domain.Enumerations.KnownProductType.InfectionTracking);

            model.SetData(infections, precautionData);

            return View(model);
        }

        public ActionResult LineListingIncident(Models.Reporting.Incident.Facility.LineListingIncidentView model)
        {
            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.IncidentGroupOptions = IncidentRepository.AllTypes.Select( x=> x.GroupName).Distinct().Select(x => new SelectListItem() { Text = x, Value = x });
            model.InjuryTypeOptions = IncidentRepository.AllInjuries.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.FloorOptions = _Facility.Floors.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            if (model.Wing < 1)
            {
                model.Wing = wings.FirstOrDefault().Id;
            }

            if(model.StartDate.HasValue == false)
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                model.EndDate = DateTime.Today;
            }

            var incidents = this.IncidentRepository.FindForLineListing(
                ActionContext.CurrentFacility,
                model.Wing,
                model.Floor,
                model.SelectedIncidentGroups,
                model.SelectedInjuryTypes,
                model.StartDate,
                model.EndDate);

            var precautionData = PrecautionRepository.GetForFacilityAndProduct(ActionContext.CurrentFacility.Id,
            Domain.Enumerations.KnownProductType.IncidentTracking);

            model.SetData(incidents, precautionData);

            return View(model);

        }

        public ActionResult PatientList(Models.Reporting.Patient.PatientListView model)
        {
            if (model.SortMode.HasValue == false)
            {
                model.SortMode = 1;
            }

            model.SortModeOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "First,Last Name", Value = "1" },
                new SelectListItem() { Text = "Last,First Name", Value = "2" },
                new SelectListItem() { Text = "Floor,Wing,Room", Value = "3" },
                new SelectListItem() { Text = "Admission Date", Value = "4" }
            };

            var patients = PatientRepository.Find(_Facility).Where(x => x.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted);

            if (model.SortMode == 1)
            {
                patients = patients.OrderBy(x => x.GetFirstName()).ThenBy(x => x.GetLastName());
            }

            if (model.SortMode == 2)
            {
                patients = patients.OrderBy(x => x.GetLastName()).ThenBy(x => x.GetFirstName());
            }

            if (model.SortMode == 3)
            {
                patients = patients.OrderBy(x => x.Room.Wing.Floor.Name).ThenBy(x => x.Room.Wing.Name).ThenBy(x => x.Room.Name);
            }

            if (model.SortMode == 4)
            {
                patients = patients.OrderByDescending(x => x.GetLastAdmissionDate());
            }

            model.Patients = patients
                .Select(x => new Models.Reporting.Patient.PatientListView.Patient()
                {
                     AdmissionDate = x.GetLastAdmissionDate().FormatAsShortDate(),
                     FirstName = x.GetFirstName(),
                     LastName = x.GetLastName(),
                     Middle = x.GetMiddleInitial(),
                     Floor = x.Room.Wing.Floor.Name,
                     Room = x.Room.Name,
                     Wing = x.Room.Wing.Name,
                     Flags = x.PatientFlags.Select(xx => xx.Name),
                     Precautions = this.PrecautionRepository.GetActiveForPatient(x.Id).Select(xx => xx.PrecautionType.Name),
                     Warnings = x.Warnings.Where(xx => xx.TriggeredOn > DateTime.Today.AddDays(-14)).Select(xx => xx.Title)
                }).ToList();


            return View(model);
        }

        public ActionResult LineListingComplaint(Models.Reporting.Complaint.Facility.LineListingComplaintView model)
        {
            var wings = _Facility.Floors.SelectMany(x => x.Wings).ToList();

            if (model.StartDate.HasValue == false)
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                model.EndDate = model.StartDate.Value.AddMonths(2);
            }

            model.WingOptions = wings
                .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.EmployeeOptions = EmployeeRepository.Find(ActionContext.CurrentFacility)
                .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.FullName })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.ComplaintTypeOptions = ComplaintRepository.AllTypes
                .Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            var complaints = ComplaintRepository.FindForLineListing(ActionContext.CurrentFacility,
                model.Wing,
                model.StartDate,
                model.EndDate,
                model.ComplaintType,
                model.Employee);


            model.SortOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Date Reported", Value = "1"},
                new SelectListItem() { Text = "Employee Name", Value = "2" },
                new SelectListItem() { Text = "Patient Name", Value = "3"},
                new SelectListItem() { Text = "Date Occurred", Value = "4"}
            };

            if (model.SortBy.HasValue == false)
            {
                model.SortBy = 1;
            }

            if (model.SortBy == 1)
            {
                complaints = complaints.OrderBy(x => x.DateReported).ToList();
            }

            if (model.SortBy == 2)
            {
                complaints = complaints.OrderBy(x => x.Employee != null ? x.Employee.FullName : string.Empty).ToList();
            }

            if (model.SortBy == 3)
            {
                complaints = complaints.OrderBy(x => x.Patient != null ? x.Patient.FullName : string.Empty).ToList();
            }


            if (model.SortBy == 4)
            {
                complaints = complaints.OrderBy(x => x.DateOccurred).ToList();
            }

            model.SetData(complaints);

            return View(model);
        }

        public ActionResult Cms802(Models.Reporting.CmsMatrix.Matrix802View model)
        {
            var patients = PatientRepository.Find(ActionContext.CurrentFacility)
                .Where(x => x.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted);

            if (model != null && model.SelectedWing.HasValue)
            {
                patients = patients.Where(x => x.Room.Wing.Id == model.SelectedWing.Value);
            }

            patients = patients.OrderBy(x => x.Room.Name);

            var entries = CmsMatrixRepository.FindActiveEntriesForFacility(ActionContext.CurrentFacility.Id).Where(x => x.Category.MatrixType.Id == (int)Domain.Enumerations.KnownCmsMatrixType.RosterCMS802);
            var categories = CmsMatrixRepository.AllCategories.Where(x => x.Active == true).Where(x => x.MatrixType.Id == (int)Domain.Enumerations.KnownCmsMatrixType.RosterCMS802);
            var wings = ActionContext.CurrentFacility.Floors.SelectMany(x => x.Wings).ToList();
            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });
            model.CurrentFacilityName = ActionContext.CurrentFacility.Name;

            model.SetData(categories, patients, entries);

            return View(model);
        }

        public ActionResult Cms672(Models.Reporting.CmsMatrix.Census672View model)
        {
            var entries = CmsMatrixRepository.FindActiveEntriesForFacility(ActionContext.CurrentFacility.Id);

            if (model != null && model.SelectedWing.HasValue)
            {
                entries = entries.Where(x => x.Patient.Room.Wing.Id == model.SelectedWing.Value);
            }

            var wings = ActionContext.CurrentFacility.Floors.SelectMany(x => x.Wings).ToList();
            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            var options = CmsMatrixRepository.AllCategories.SelectMany(x => x.Options).ToList();

            model.SetData(entries,options);

            return View(model);
        }

        public ActionResult CmsNotes(Models.Reporting.CmsMatrix.NotesView model)
        {
            var entries = CmsMatrixRepository.FindActiveNotesForFacility(ActionContext.CurrentFacility.Id);

            entries = entries.OrderBy(x => x.Patient.Room.Name);

            if (model != null && model.SelectedWing.HasValue)
            {
                entries = entries.Where(x => x.Patient.Room.Wing.Id == model.SelectedWing.Value);
            }

            var wings = ActionContext.CurrentFacility.Floors.SelectMany(x => x.Wings).ToList();
            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            var options = CmsMatrixRepository.AllCategories.SelectMany(x => x.Options).ToList();

            model.SetData(entries);

            return View(model);
        }

        public ActionResult AntibioticUtilizationMultipleByMD(Models.Reporting.Infection.Facility.AntibioticUtilizationMultipleByMD model)
        {
            if (model.StartDate.IsNullOrEmpty())
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1).FormatAsShortDate();
                model.EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1).FormatAsShortDate();
            }

            var infectionTypes = new List<SelectListItem>();
            infectionTypes.Add(new SelectListItem() { Text = "Show All", Value = string.Empty });

            foreach (var infectionType in InfectionRepository.AllInfectionTypes.Where(x => x.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id))
            {
                infectionTypes.Add(new SelectListItem() { Text = infectionType.Name, Value = infectionType.Id.ToString() });
            }


            model.InfectionTypeOptions = infectionTypes;

            DateTime startDate;
            DateTime endDate;

            DateTime.TryParse(model.StartDate, out startDate);
            DateTime.TryParse(model.EndDate, out endDate);

            var infections = InfectionRepository.FindTreatments(ActionContext.CurrentFacility, startDate, endDate, model.InfectionTypeId);

            model.SetData(infections);

            return View(model);

        }

        public ActionResult AntibioticUtilizationByMDStatistics(Models.Reporting.Infection.Facility.AntibioticUtilizationByMDStatistics model)
        {
            if (model.StartDate.IsNullOrEmpty())
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1).FormatAsShortDate();
                model.EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1).FormatAsShortDate();
            }

            var infectionTypes = new List<SelectListItem>();
            infectionTypes.Add(new SelectListItem() { Text = "Show All", Value = string.Empty });

            foreach (var infectionType in InfectionRepository.AllInfectionTypes.Where(x => x.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id))
            {
                infectionTypes.Add(new SelectListItem() { Text = infectionType.Name, Value = infectionType.Id.ToString() });
            }


            model.InfectionTypeOptions = infectionTypes;

            DateTime startDate;
            DateTime endDate;

            DateTime.TryParse(model.StartDate, out startDate);
            DateTime.TryParse(model.EndDate, out endDate);

            var infections = InfectionRepository.FindForLineListing(ActionContext.CurrentFacility, null, null, true,
               startDate, endDate, model.InfectionTypeId, false, null, null, null);

            
            model.SetData(infections);

            return View(model); 

        }

        public ActionResult LineListingPsychotropic(Models.Reporting.Psychotropic.Facility.LineListingPsychotropicView model)
        {
            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            if (model.StartDate.HasValue == false)
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }

            if (model.EndDate.HasValue == false)
            {
                model.EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
            }

            model.DrugTypeOptions = PsychotropicRespository
                .AllDrugTypes
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                .Prepend(new SelectListItem() { Text = "All", Value = string.Empty } );


            var data = PsychotropicRespository.FindForLineListing(ActionContext.CurrentFacility,
                model.Wing,
                model.StartDate.Value,
                model.EndDate.Value,
                model.ActiveOnly);

            if (model.DrugType.HasValue)
            {
                data = data.Where(x => x.DrugType.Id == model.DrugType.Value);
            }

            model.SetDate(data, PsychotropicRespository.AllDrugTypes, PsychotropicRespository.AllFrequencies);

            return View(model);
        }

        public ActionResult LabDailyLineListing(Models.Reporting.Infection.Facility.LabDailyLineListingView model)
        {

            var infectionTypes = new List<SelectListItem>();
            infectionTypes.Add(new SelectListItem() { Text = "Show All", Value = string.Empty });

            foreach (var infectionType in InfectionRepository.AllInfectionTypes.Where(x => x.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id))
            {
                infectionTypes.Add(new SelectListItem() { Text = infectionType.Name, Value = infectionType.Id.ToString() });
            }


            var infections = this.InfectionRepository.FindForLineListing(
                ActionContext.CurrentFacility,
                null,
                null,
                false,
                null,
                null,
                null,
                false,
                null,
                null,
                null);

            model.SetData(infections);

            return View(model);
        }

        public ActionResult LineListingWound(Models.Reporting.Wound.Facility.LineListingWoundView model)
        {
            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.FloorOptions = _Facility.Floors.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name }).Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.TypeOptions = WoundRepository.AllTypes.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.StageOptions = WoundRepository.AllStages.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            var progressNames = System.Enum.GetNames(typeof(Domain.Enumerations.WoundProgress));

            var progressOptions = new List<SelectListItem>();
            progressOptions.Add(new SelectListItem() { Text = "Show All", Value = string.Empty });

            foreach (var name in progressNames)
            {
                var progressValue = (int)System.Enum.Parse(typeof(Domain.Enumerations.WoundProgress), name);
                progressOptions.Add(new SelectListItem() { Text = name.SplitPascalCase(), Value = progressValue.ToString() });
            }

            model.MostRecentProgressOptions = progressOptions;


            model.ModeOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Active Only", Value = Models.Reporting.Wound.Facility.LineListingWoundView.MODE_ACTIVE_ONLY.ToString() },
                new SelectListItem() { Text = "All", Value = Models.Reporting.Wound.Facility.LineListingWoundView.MODE_ALL.ToString() },
                new SelectListItem() { Text = "Resolved Only", Value = Models.Reporting.Wound.Facility.LineListingWoundView.MODE_INACTIVE_ONLY.ToString() }
            };

            var classificationNames = System.Enum.GetNames(typeof(Domain.Enumerations.WoundClassification));

            var classificationOptions = new List<SelectListItem>();
            classificationOptions.Add(new SelectListItem() { Text = "Show All", Value = string.Empty });

            foreach (var name in classificationNames)
            {
                var classificationValue = (int)System.Enum.Parse(typeof(Domain.Enumerations.WoundClassification), name);
                classificationOptions.Add(new SelectListItem() { Text = name.SplitPascalCase(), Value = classificationValue.ToString() });
            }

            model.ClassificationOptions = classificationOptions;

            bool includeResolved = false;
            bool includeResolvedOnly = false;

            if (model.Mode == Models.Reporting.Wound.Facility.LineListingWoundView.MODE_ACTIVE_ONLY)
            {
                includeResolved = false;
                includeResolvedOnly = false;
            }
            else if (model.Mode == Models.Reporting.Wound.Facility.LineListingWoundView.MODE_ALL)
            {
                includeResolved = true;
                includeResolvedOnly = false;
            }
            else if (model.Mode == Models.Reporting.Wound.Facility.LineListingWoundView.MODE_INACTIVE_ONLY)
            {
                includeResolved = true;
                includeResolvedOnly = true;
            }

            var data = WoundRepository.FindForLineListing(ActionContext.CurrentFacility,
                model.Wing,
                model.Floor,
                includeResolved,
                includeResolvedOnly,
                model.StartDate,
                model.EndDate,
                model.CurrentStage,
                model.Classification,
                model.Type);

            var precautionData = PrecautionRepository.GetForFacilityAndProduct(ActionContext.CurrentFacility.Id,
            Domain.Enumerations.KnownProductType.WoundTracking);

            model.SetData(data, model.MostRecentProgress, precautionData);

            return View(model);

        }

        public ActionResult LineListingVaccine(Models.Reporting.Vaccine.Facility.LineListingVaccineView model)
        {
            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.TypeOptions = VaccineRepository.AllVaccineTypes
                .OrderBy(x => x.CVXShortDescription)
                .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.CVXShortDescription })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.RefusalOptions = VaccineRepository.AllVaccineRefusalReasons.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.CodeValue })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            model.SortOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "By Patient Name", Value = "1" },
                new SelectListItem() { Text = "By Room", Value = "2" },
            };

            var data = VaccineRepository.FindForLineListing(ActionContext.CurrentFacility,
                model.Wing,
                model.VaccineTypeId,
                model.VaccineRefusalReasonId,
                model.StartDate,
                model.EndDate);

            var patients = PatientRepository.Find(ActionContext.CurrentFacility);


            if(!model.IncludeDischarged)
            {
                patients = patients.Where(x => x.CurrentStatus == Domain.Enumerations.PatientStatus.Admitted);
            }

            if(model.Wing.HasValue)
            {
                patients = patients.Where(x => x.Room.Wing.Id == model.Wing.Value);
            }

            if(model.VaccineTypeId.HasValue || model.VaccineRefusalReasonId.HasValue )
            {
                patients = patients.Where(x => data.Select(xx=> xx.Patient.Id).Contains(x.Id));
            }

            if(model.SortId == null || model.SortId == 1)
            {
                patients = patients.OrderBy(x => x.FullName);
            }
            else
            {
                patients = patients.OrderBy(x => x.Room.Name);
            }

            model.SetData(data, patients);

            return View(model);

        }

        public ActionResult OrganismsByYear(Models.Reporting.Infection.Facility.OrganismYearly model)
        {
            if (model.Year.HasValue == false || model.Year < 2000 || model.Year > 2050)
            {
                model.Year = DateTime.Today.Year;
            }

            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });

            var startDate = new DateTime(model.Year.Value,1,1);
            var endDate = startDate.AddYears(1).AddDays(-1);

            var data = LabResultRepository.FindPathogens(ActionContext.CurrentFacility.Id, startDate, endDate, model.WingId);

            model.SetData(data);

            return View(model);
        }

        public ActionResult LineListingCatheter(Models.Reporting.Catheter.Facility.LineListingCatheterView model)
        {
            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });


            var data = CatheterRepository.FindForLineListing(ActionContext.CurrentFacility,
                model.Wing,
                model.StartDate,
                model.EndDate,
                model.IncludeDiscontinued);

            model.SetData(data);

            return View(model);

        }

        public ActionResult PatientRepeatFalls(Models.Reporting.Incident.Facility.PatientRepeatFallView model)
        {
            if (model.StartDate.HasValue == false)
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0);
            }

            if (model.EndDate.HasValue == false)
            {
                model.EndDate = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
            }

            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });


            var results = IncidentRepository.FindForLineListing(_Facility, model.Wing, null,
                new List<string>(new string[] { "Fall" }), null,model.StartDate,model.EndDate);

            model.SetData(results);

            return View(model);
        }

        public ActionResult FallsByCNA(Models.Reporting.Incident.Facility.CNAFallView model)
        {
            if (model.StartDate.HasValue == false)
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0);
            }

            if (model.EndDate.HasValue == false)
            {
                model.EndDate = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
            }

            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });


            var results = IncidentRepository.FindForLineListing(_Facility, model.Wing, null,
                new List<string>(new string[] { }),null, model.StartDate, model.EndDate);

            model.SetData(results);

            return View(model);
        }

        public ActionResult FallsByWitnessed(Models.Reporting.Incident.Facility.WitnessedFallView model)
        {
            if (model.StartDate.HasValue == false)
            {
                model.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0);
            }

            if (model.EndDate.HasValue == false)
            {
                model.EndDate = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
            }

            var wings = new List<Domain.Models.Wing>();

            foreach (var room in _Facility.Floors)
            {
                foreach (var wing in room.Wings)
                {
                    wings.Add(wing);
                }
            }

            model.WingOptions = wings.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = string.Concat(m.Floor.Name, " - ", m.Name) })
                .Prepend(new SelectListItem() { Text = "Show All", Value = string.Empty });


            var results = IncidentRepository.FindForLineListing(_Facility, model.Wing, null,
                new List<string>(new string[] { "Fall"  }), null, model.StartDate, model.EndDate);

            model.SetData(results);

            return View(model);
        }

        public ActionResult WoundFlowSheet(Models.Reporting.Wound.Facility.WeeklyFlowSheet model)
        {

            var data = WoundRepository.FindForLineListing(ActionContext.CurrentFacility,
                null,
                null,
                false,
                false,
                null,
                null,
                null,
                null,
                null);

            model.SetData(data);

            return View(model);

        }

    }


}
