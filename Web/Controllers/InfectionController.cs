using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.ObjectModel.AuditTracking;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Infection;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using IQI.Intuition.Web.Models.Reporting.Infection.Facility;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class InfectionController : Controller
    {
        public InfectionController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IInfectionRepository infectionRepository,
            ILabResultRepository labResultRepository,
            ITreatmentRepository treatmentRepository,
            IPatientRepository patientRepository,
            IPrecautionRepository precautionRepository,
            AuditTrackingWorker auditWorker)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            InfectionRepository = infectionRepository.ThrowIfNullArgument("infectionRepository");
            LabResultRepository = labResultRepository.ThrowIfNullArgument("labResultRepository");
            TreatmentRepository = treatmentRepository.ThrowIfNullArgument("treatmentRepository");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            PrecautionRepository = precautionRepository.ThrowIfNullArgument("precautionRepository");
            AuditWorker = auditWorker;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IInfectionRepository InfectionRepository { get; private set; }
        protected virtual ILabResultRepository LabResultRepository { get; private set; }
        protected virtual ITreatmentRepository TreatmentRepository { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual IPrecautionRepository PrecautionRepository { get; private set; }
        protected virtual AuditTrackingWorker AuditWorker { get; private set; }

        [HttpGet]
        public ActionResult List(bool? showOpenOnly)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("ListData", new { showOpenOnly = (showOpenOnly ?? false) });
            var gridModel = new InfectionGrid(dataUrl);

            // By convention, we normally use a dedicated model class for each view but, since this view 
            //  does not require anything more than the grid itself, we are using the grid model instead
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(InfectionGridRequest requestModel)
        {
            var infectionQuery = InfectionRepository.Find(
                ActionContext.CurrentFacility,
                requestModel.PatientFullName,
                requestModel.RoomAndWingName,
                requestModel.InfectionSiteTypeName,
                requestModel.FirstNotedOn,
                requestModel.ReasonForEntry,
                requestModel.XrayDate,
                requestModel.LabResultsText,
                requestModel.TreatementText,
                requestModel.ResolvedOn,
                !requestModel.ShowOpenOnly,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new InfectionGrid(infection => Url.Action("Edit", new { id = infection.Id }));
            ModelMapper.MapForReadOnly(infectionQuery, gridModel);

            return gridModel.GetJsonResult();
        }

        [HttpGet]
        public ActionResult PatientInfectionList(Guid? id)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("PatientInfectionListData", new { PatientGuid = id });
            var gridModel = new PatientInfectionGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult PatientInfectionListData(InfectionGridRequest requestModel)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(requestModel.PatientGuid ?? Guid.Empty);

            if (patient == null)
            {
                return null;
            }

            var infectionQuery = InfectionRepository.Find(
                patient,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new PatientInfectionGrid(infection =>
                Url.Action("Edit", new 
                { 
                    id = infection.Id,
                    returnUrl = Url.Action("Detail", "Patient", new { id = requestModel.PatientGuid }) 
                }));
            ModelMapper.MapForReadOnly(infectionQuery, gridModel);

            return gridModel.GetJsonResult();
        }

        [HttpGet]
        public ActionResult Add(Guid? id)
        {
            if (id.HasValue)
            {
                var patient = ActionContext.CurrentFacility.FindPatient(id.Value);

                if (patient != null)
                {
                    var types = InfectionRepository.AllInfectionTypes.Where
                        (x => x.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id && x.IsHidden != true);


                    var sites = InfectionRepository.AllInfectionSites.Where
                        (x => x.Type.IsHidden != true && x.Type.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id);

                    var formModel = ModelMapper.MapForCreate<InfectionForm>();
                    formModel.Room = patient.Room.Id;
                    formModel.Wing = patient.Room.Wing.Id;
                    formModel.Floor = patient.Room.Wing.Floor.Id;
                    formModel.DefaultMD = patient.MDName;
                    formModel.ClientData.InfectionTypes = types.Select(type => new { Text = type.Name, Value = type.Id }).ToArray();
                    formModel.CriteriaDefinition = ActionContext.CurrentFacility.InfectionDefinition.Name;
                    formModel.ClientData.InfectionSites = GenerateInfectionSites(sites);

                    // Since we are using a read-only mapping for patient info we have to manually map this property
                    ModelMapper.ReadFromDomain(patient, formModel.Patient);

                    return View("Edit", formModel);
                }
            }

            return RedirectToAction("List");
        }

        [HttpPost, SupportsFormCancel, ValidateInput(false)]
        public ActionResult Add(InfectionForm formModel, bool formCancelled)
        {
            Patient patient = null;

            try
            {
                if (formCancelled != true)
                {
                    patient = ActionContext.CurrentFacility.FindPatient(formModel.Patient.Guid);

                    if (patient == null)
                    {
                        return RedirectToAction("List");
                    }

                    var infection = new InfectionVerification(patient);
                    ModelMapper.MapForCreate(formModel, infection);
                    InfectionRepository.Add(infection);
                }

                // Since Add is only performed from the patient chart screen, return the user to that screen
                return RedirectToAction("Detail", new { controller = "Patient", id = formModel.Patient.Guid });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            // Must refresh the these child models before the view result is returned
            ModelMapper.ReadFromDomain(patient, formModel.Patient);
            ModelMapper.AssignDefaults(formModel.ClientData);

            var types = InfectionRepository.AllInfectionTypes.Where
                (x => x.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id && x.IsHidden != true);

            var sites = InfectionRepository.AllInfectionSites.Where
                (x => x.Type.IsHidden != true && x.Type.Definition.Id == ActionContext.CurrentFacility.InfectionDefinition.Id);

            formModel.ClientData.InfectionTypes = types.Select(type => new { Text = type.Name, Value = type.Id }).ToArray();
            formModel.CriteriaDefinition = ActionContext.CurrentFacility.InfectionDefinition.Name;
            formModel.ClientData.InfectionSites = GenerateInfectionSites(sites);

            foreach (var type in formModel.InfectionTreatmentTypes)
            {
                type.TreatmentNames = TreatmentRepository.AllTreatments.Where(x => x.TreatmentType.Id == type.Id).Select(x => x.Name).ToList();
            }
            

            return View("Edit", formModel);
        }

        [SupportsTokenAuthentication]
        public ActionResult View(int? id, string returnUrl)
        {
            var infection = InfectionRepository.Get(id ?? 0);

            if (infection == null || infection.Patient.Account != ActionContext.CurrentAccount)
            {
                return RedirectToAction("List");
            }

            var precautions = PrecautionRepository.GetActiveForPatient(infection.Patient.Id);
            var formModel = new LineListingInfectionView.InfectionRow(infection, precautions);
            return View(formModel);
        }

        [HttpPost]
        public ActionResult View(int? id, string returnUrl, FormCollection data)
        {
            if (returnUrl.IsNullOrEmpty())
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(returnUrl);
        }


        [HttpGet, SupportsTokenAuthentication]
        public ActionResult Edit(int? id, string returnUrl)
        {
            this.ViewData["ExportPath"] = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri.Replace("Edit", "View");
            this.ViewData["IsExportable"] = true;
            this.ViewData["LimitedExport"] = true;


            var infection = InfectionRepository.Get(id ?? 0);

            var definition = infection.InfectionSite.Type.Definition;
            var types = InfectionRepository.AllInfectionTypes.Where
                (x => x.Definition.Id == definition.Id && x.IsHidden != true);

            var sites = InfectionRepository.AllInfectionSites.Where
                (x => x.Type.IsHidden != true && x.Type.Definition.Id == definition.Id);

            if (infection == null || infection.Patient.Account != ActionContext.CurrentAccount)
            {
                return RedirectToAction("List");
            }

            if (infection.IsResolved == true && ActionContext.CurrentUser.HasPermission(Domain.Enumerations.KnownPermision.EditResolvedInfections) == false)
            {
                return RedirectToAction("View", new { id = id, returnUrl = returnUrl });
            }

            var formModel = ModelMapper.MapForUpdate<InfectionForm>(infection);
            formModel.ClientData.InfectionTypes = types.Select(type => new { Text = type.Name, Value = type.Id }).ToArray();
            formModel.ClientData.InfectionSites = GenerateInfectionSites(sites);
            formModel.CriteriaDefinition = definition.Name;


            // Since we are using a read-only mapping for patient info we have to manually map this property
            ModelMapper.ReadFromDomain(infection.Patient, formModel.Patient);
            formModel.DefaultMD = infection.Patient.MDName;

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel, ValidateInput(false)]
        public ActionResult Edit(InfectionForm formModel, bool formCancelled, string returnUrl)
        {
            this.ViewData["ExportPath"] = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri.Replace("Edit", "View");
            this.ViewData["IsExportable"] = true;

            try
            {
                if (formCancelled != true)
                {
                    var infection = InfectionRepository.Get(formModel.InfectionVerificationId ?? 0);
                     
                    if (infection == null || infection.Patient.Account != ActionContext.CurrentAccount)
                    {
                        return RedirectToAction("List");
                    }

                    if (infection != null)
                    {

                        var definition = infection.InfectionSite.Type.Definition;
                        var types = InfectionRepository.AllInfectionTypes.Where
                            (x => x.Definition.Id == definition.Id && x.IsHidden != true);

                        var sites = InfectionRepository.AllInfectionSites.Where
                            (x => x.Type.IsHidden != true && x.Type.Definition.Id == definition.Id);

                        formModel.ClientData.InfectionTypes = types.Select(type => new { Text = type.Name, Value = type.Id }).ToArray();
                        formModel.ClientData.InfectionSites = GenerateInfectionSites(sites);
                        
                        formModel.CriteriaDefinition = definition.Name;

                        ModelMapper.MapForUpdate(formModel, infection);


                        AuditWorker.AuditOnUpdate(infection);
                    }
                }

                if (returnUrl.IsNotNullOrWhiteSpace())
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("List");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            var patient = ActionContext.CurrentFacility.FindPatient(formModel.Patient.Guid);

            if (patient == null)
            {
                return RedirectToAction("List");
            }

            // Must refresh the these child models before the view result is returned
            ModelMapper.ReadFromDomain(patient, formModel.Patient);
            ModelMapper.AssignDefaults(formModel.ClientData);

            foreach (var type in formModel.InfectionTreatmentTypes)
            {
                type.TreatmentNames = TreatmentRepository.AllTreatments.Where(x => x.TreatmentType.Id == type.Id).Select(x => x.Name).ToList();
            }

            return View(formModel);
        }

        public ActionResult CreateLabResult(int testId, string competedOn, int resultId)
        {
            var test = LabResultRepository.AllTestTypes.Where( x => x.Id == testId).FirstOrDefault();
            var result = LabResultRepository.AllResults.Where(x => x.Id == resultId).FirstOrDefault();

            var data = new Web.Models.Infection.InfectionLabResult();
            data.LabTestTypeId = testId;
            data.LabTestTypeName = test.Name;
            data.Removed = false;
            data.LabCompletedOn = competedOn;
            data.LabResultId = resultId;
            data.LabResultName = result.Name;
            data.Notes = string.Empty;
            data.Pathogens = new List<Web.Models.Infection.InfectionLabResultPathogen>();
            

            if (result.Positive && test.PathogenSet != null)
            {
                data.PathogenOptions = test.PathogenSet.Pathogens
                    .Select(x => new Web.Models.Option() { Id = x.Id, Name = x.Name })
                    .OrderBy(x => x.Name)
                    .Prepend(new Web.Models.Option[] { new Web.Models.Option() { Id = -1, Name = "Select..." } })
                    .ToList();
            }
            else
            {
                data.PathogenOptions = new List<Web.Models.Option>();
            }

            if (result.Positive && test.PathogenQuantitySet != null)
            {
                data.PathogenQuantityOptions = test.PathogenQuantitySet.PathogenQuantities.Select(x => new Web.Models.Option() { Id = x.Id, Name = x.Name }).ToList();
            }
            else
            {
                data.PathogenQuantityOptions = new List<Web.Models.Option>();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApplicableLabResults(int testId)
        {
            var test = LabResultRepository.AllTestTypes.Where(x => x.Id == testId).FirstOrDefault();
            var results = test.LabResultSet.LabResults.Select(x => new { Id = x.Id, Name = x.Name }).ToArray();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSiteDetails(int id, string patient, int? infectionId)
        {
            var site = InfectionRepository.AllInfectionSites.Where(x => x.Id == id).FirstOrDefault();

            var details = site.SupportingDetails;
            var description = site.SupportingDetailsDescription;
            var items = details.Select(detail => new
                    {
                        Text = detail.Name,
                        Value = detail.Id,
                        InfectionSite = detail.InfectionSite.Id,
                        Selected = false
                    }).OrderBy(x => x.Text);

            bool hideCriteria = false;
            string hiddenCriteriaMessage = string.Empty;

            if (site.CriteriaAvailabilityRule == Domain.Enumerations.InfectionCriteriaAvailabilityRule.Cdiff)
            {
                var query = InfectionRepository.FindForPatient(new Guid(patient), null, id)
                    .Where(x => x.Classification == InfectionClassification.HealthCareAssociatedInfection)
                    .Where(x => x.FirstNotedOn >= DateTime.Today.AddDays(-56))
                    .Where( x=> x.Deleted == false || x.Deleted == null);


                if (infectionId.HasValue)
                {
                    query = query.Where(x => x.Id != infectionId);
                }

                var match = query.FirstOrDefault();


                if (match != null)
                {
                    hideCriteria = true;
                    hiddenCriteriaMessage = string.Format("A C. diff recurrence within the last 8 weeks ({0}) was identified.", match.FirstNotedOn.FormatAsShortDate());
                }
            }


            return Json(
                new { 
                    items = items, 
                    description = description,
                    hideCriteria = hideCriteria,
                    hiddenCriteriaMessage = hiddenCriteriaMessage
                }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Remove(int id)
        {
            var infection = InfectionRepository.Get(id);

            if (infection.Patient.Room.Wing.Floor.Facility.Id != ActionContext.CurrentFacility.Id)
            {
                throw new Exception("Patient does not belong to this account");
            }

            infection.Deleted = true;

            return RedirectToAction("Detail", "Patient", new { id = infection.Patient.Guid });
            
        }

        public ActionResult ListByRoom(Guid roomGuid, DateTime startDate, DateTime endDate, int displayMode)
        {
            var room = ActionContext.CurrentFacility.Floors.SelectMany(x => x.Wings).SelectMany(x => x.Rooms).Where(x => x.Guid == roomGuid).FirstOrDefault();

            var infections = InfectionRepository.FindForLineListing(
                ActionContext.CurrentFacility,null, null, true, startDate, endDate, null, false, null, null, null);

            infections = infections.Where(x => x.Room.Id == room.Id);

            if (displayMode == 1)
            {
                infections.Where(x => x.Classification != InfectionClassification.NoInfection);
            }
            else if (displayMode == 2)
            {
                infections.Where(x => x.Classification == InfectionClassification.HealthCareAssociatedInfection);
            }
            else if (displayMode == 3)
            {
                infections.Where(x => x.Classification == InfectionClassification.Admission || x.Classification == InfectionClassification.AdmissionHospitalDiagnosed);
            }

            var results = new ArrayList();

            foreach (var infection in infections)
            {
                results.Add(new
                {
                    Patient = infection.Patient.FullName,
                    FirstNoted = infection.FirstNotedOn.FormatAsShortDate(),
                    Condition = infection.InfectionSite.Name
                });
            }

            return Json(new { Room = room.Name, Infections = results }, JsonRequestBehavior.AllowGet);
        }


        private IEnumerable GenerateInfectionSites(IEnumerable<InfectionSite> src)
        {
            var data = src.Select(x => new
            {
                Text = x.Name,
                Value = x.Id,
                InfectionType = x.Type.Id,
                DetailsDescription = x.SupportingDetailsDescription,
                RuleSet = GenerateRuleSet(x.RuleSet)
            }).ToArray();

            return data;
        }

        public object GenerateRuleSet(InfectionCriteriaRuleSet src)
        {
            return new
            {
                Comments = src.CommentsText,
                Instructions = src.InstructionsText,
                MinimumRules = src.MinimumRuleCount,
                RuleSetId = src.Id,
                Rules = src.CriteriaRules
                    .Select(rule => GenerateRule(rule))
                    .ToArray(),
                RuleSets = src.ChildCriteriaRuleSets
                    .Select(ruleSet => GenerateRuleSet(ruleSet))
                    .ToArray()
            };
        }

        public object GenerateRule(InfectionCriteriaRule src)
        {
            return new
            {
                Instructions = src.InstructionsText,
                MinimumCriteria = src.MinimumCriteriaCount,
                Criteria = src.CriteriaOptions
                    .Select(criteria => new
                    {
                        Name = criteria.Name,
                        Id = criteria.Id
                    })
                    .ToArray()
            };
        }

    }
}
