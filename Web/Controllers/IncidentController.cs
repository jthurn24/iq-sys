using System;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.ObjectModel.AuditTracking;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Incident;
using IQI.Intuition.Web.Attributes;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount, EnableAccountRestriction]
    public class IncidentController : Controller
    {
        public IncidentController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IIncidentRepository incidentRepository,
            IPrecautionRepository precautionRepository,
            AuditTrackingWorker auditWorker
            )
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            IncidentRepository = incidentRepository.ThrowIfNullArgument("incidentRepository");
            PrecautionRepository = precautionRepository.ThrowIfNullArgument("precautionRepository");
            AuditWorker = auditWorker;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IIncidentRepository IncidentRepository { get; private set; }

        protected virtual AuditTrackingWorker AuditWorker { get; private set; }

        protected virtual IPrecautionRepository PrecautionRepository { get; private set; }

        [HttpGet]
        public ActionResult List()
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("ListData");
            var gridModel = new IncidentGrid(dataUrl);

            // By convention, we normally use a dedicated model class for each view but, since this view 
            //  does not require anything more than the grid itself, we are using the grid model instead
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(IncidentGridRequest requestModel)
        {
            var incidentQuery = IncidentRepository.Find(
                ActionContext.CurrentFacility,
                requestModel.PatientFullName,
                requestModel.RoomAndWingName,
                requestModel.IncidentTypes,
                requestModel.DiscoveredOn,
                requestModel.OccurredOn,
                requestModel.InjuryLevel,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new IncidentGrid(i => Url.Action("Edit", new { id = i.Id }));
            ModelMapper.MapForReadOnly(incidentQuery, gridModel);

            return gridModel.GetJsonResult();
        }

        [HttpGet]
        public ActionResult PatientIncidentList(Guid? id)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("PatientIncidentListData", new { PatientGuid = id });
            var gridModel = new PatientIncidentGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult PatientIncidentListData(IncidentGridRequest requestModel)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(requestModel.PatientGuid ?? Guid.Empty);

            if (patient == null)
            {
                return null;
            }

            var IncidentQuery = IncidentRepository.Find(
                patient,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new PatientIncidentGrid(i =>
                Url.Action("Edit", new 
                { 
                    id = i.Id,
                    returnUrl = Url.Action("Detail", "Patient", new { id = requestModel.PatientGuid }) 
                }));
            ModelMapper.MapForReadOnly(IncidentQuery, gridModel);

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
                    var formModel = ModelMapper.MapForCreate<IncidentForm>();
                    formModel.Room = patient.Room.Id;
                    formModel.Wing = patient.Room.Wing.Id;
                    formModel.Floor = patient.Room.Wing.Floor.Id;
                    formModel.DiscoveredOn = DateTime.Now;
                    formModel.DiscoveredHour = DateTime.Now.Hour;
                    formModel.DiscoveredMinutes = DateTime.Now.Minute;
                    formModel.OccurredOn = DateTime.Now;
                    formModel.OccurredHour = DateTime.Now.Hour;
                    formModel.OccurredMinutes = DateTime.Now.Minute;

                    // Since we are using a read-only mapping for patient info we have to manually map this property
                    ModelMapper.ReadFromDomain(patient, formModel.Patient);

                    return View("Edit", formModel);
                }
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Add(IncidentForm formModel, bool? cancel)
        {
            Patient patient = null;

            try
            {
                if (cancel.IsNotTrue())
                {
                    patient = ActionContext.CurrentFacility.FindPatient(formModel.Patient.Guid);

                    if (patient == null)
                    {
                        return RedirectToAction("List");
                    }

                    var incident = new IncidentReport(patient);
                    ModelMapper.MapForCreate(formModel, incident);
                    IncidentRepository.Add(incident);
                }

                // Since Add is only performed from the patient chart screen, return the user to that screen
                return RedirectToAction("Detail", new { controller = "Patient", id = formModel.Patient.Guid });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            // Must refresh the these child models before the view result is returned
            ModelMapper.AssignDefaults(formModel.ClientData);

            return View("Edit", formModel);
        }


        [HttpGet, SupportsTokenAuthentication]
        public ActionResult Edit(int? id, string returnUrl)
        {
            this.ViewData["ExportPath"] = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri.Replace("Edit", "View");
            this.ViewData["IsExportable"] = true;
            this.ViewData["LimitedExport"] = true;


            var incident = IncidentRepository.Get(id ?? 0);

            if (incident == null || incident.Patient.Account != ActionContext.CurrentAccount)
            {
                return RedirectToAction("List");
            }

            if (incident.AssessmentCompleted == true && ActionContext.CurrentUser.HasPermission(Domain.Enumerations.KnownPermision.EditResolvedIncidents) == false)
            {
                return RedirectToAction("View", new { id = id, returnUrl = returnUrl });
            }

            var formModel = ModelMapper.MapForUpdate<IncidentForm>(incident);

            // Since we are using a read-only mapping for patient info we have to manually map this property
            ModelMapper.ReadFromDomain(incident.Patient, formModel.Patient);

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(IncidentForm formModel, bool formCancelled, string returnUrl)
        {
            this.ViewData["ExportPath"] = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri.Replace("Edit", "View");
            this.ViewData["IsExportable"] = true;

            try
            {
                if (formCancelled != true)
                {
                    var incident = IncidentRepository.Get(formModel.IncidentReportId ?? 0);

                    if (incident != null)
                    {
                        ModelMapper.MapForUpdate(formModel, incident);
                        AuditWorker.AuditOnUpdate(incident);
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

            return View(formModel);
        }

        [SupportsTokenAuthentication]
        public ActionResult View(int? id, string returnUrl)
        {
            var incident = IncidentRepository.Get(id ?? 0);

            var precautions = PrecautionRepository.GetActiveForPatient(incident.Patient.Id);
            var formModel = new IQI.Intuition.Web.Models.Reporting.Incident.Facility.LineListingIncidentView.IncidentRow(incident, precautions);
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


        public ActionResult Remove(int id)
        {
            var incident = IncidentRepository.Get(id);
            incident.Deleted = true;

            return RedirectToAction("Detail", "Patient", new { id = incident.Patient.Guid });

        }


        public ActionResult ListByRoom(Guid roomGuid, DateTime startDate, DateTime endDate)
        {
            var room = ActionContext.CurrentFacility.Floors.SelectMany(x => x.Wings).SelectMany(x => x.Rooms).Where(x => x.Guid == roomGuid).FirstOrDefault();

            var incidents = IncidentRepository.FindForLineListing(ActionContext.CurrentFacility, null,null, null, null, startDate, endDate);

            incidents = incidents.Where(x => x.Room.Id == room.Id);


            var results = new ArrayList();

            foreach (var incident in incidents)
            {
                results.Add(new
                {
                    Patient = incident.Patient.FullName,
                    On = incident.OccurredOn.HasValue ? incident.OccurredOn.Value.ToString("MM/dd/yyyy") : incident.DiscoveredOn.Value.ToString("MM/dd/yyyy"),
                    Group = incident.IncidentTypes.Select(x => x.GroupName).Distinct().ToDelimitedString(',')
                });
            }

            return Json(new { Room = room.Name, Incidents = results }, JsonRequestBehavior.AllowGet);
        }

    }
}