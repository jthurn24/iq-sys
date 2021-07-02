using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.ObjectModel.AuditTracking;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Web.Models.Vaccine;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class VaccineController : Controller
    {
        public VaccineController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            IVaccineRepository vaccineRepository,
            AuditTrackingWorker auditWorker)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            VaccineRepository = vaccineRepository.ThrowIfNullArgument("vaccineRepository");
            AuditWorker = auditWorker;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IVaccineRepository VaccineRepository { get; private set; }
        protected virtual AuditTrackingWorker AuditWorker { get; private set; }

        [HttpGet]
        public ActionResult List()
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("ListData");
            var gridModel = new VaccineGrid(dataUrl);

            // By convention, we normally use a dedicated model class for each view but, since this view 
            //  does not require anything more than the grid itself, we are using the grid model instead
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(VaccineGridRequest requestModel)
        {
            var vaccineQuery = VaccineRepository.Find(
                ActionContext.CurrentFacility,
                requestModel.PatientFullName,
                requestModel.RoomAndWingName,
                requestModel.VaccineType,
                requestModel.AdministeredOn,
                requestModel.RefusalReason,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new VaccineGrid(vaccine => Url.Action("Edit", new { id = vaccine.Id }));
            ModelMapper.MapForReadOnly(vaccineQuery, gridModel);

            return gridModel.GetJsonResult();
        }

        [HttpGet]
        public ActionResult VaccineTradeName(int type)
        {
            return PartialView("_VaccineTradeName", VaccineRepository.AllVaccineTradeNamesForType(type));
        }

        [HttpGet]
        public ActionResult PatientVaccineList(Guid? id)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("PatientVaccineListData", new { PatientGuid = id });
            var gridModel = new PatientVaccineGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult PatientVaccineListData(VaccineGridRequest requestModel)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(requestModel.PatientGuid ?? Guid.Empty);

            if (patient == null)
            {
                return null;
            }

            var vaccineQuery = VaccineRepository.Find(
                patient,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new PatientVaccineGrid(infection =>
                Url.Action("Edit", new
                {
                    id = infection.Id,
                    returnUrl = Url.Action("Detail", "Patient", new { id = requestModel.PatientGuid })
                }));
            ModelMapper.MapForReadOnly(vaccineQuery, gridModel);

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
                    var formModel = ModelMapper.MapForCreate<VaccineForm>();
                    formModel.Room = patient.Room.Id;
                    formModel.Wing = patient.Room.Wing.Id;
                    formModel.Floor = patient.Room.Wing.Floor.Id;

                    // Since we are using a read-only mapping for patient info we have to manually map this property
                    ModelMapper.ReadFromDomain(patient, formModel.Patient);

                    return View("Edit", formModel);
                }
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Add(VaccineForm formModel, bool? cancel)
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

                    var vaccination = new VaccineEntry(patient);
                    ModelMapper.MapForCreate(formModel, vaccination);
                    VaccineRepository.Add(vaccination);
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


            var vaccination = VaccineRepository.Get(id ?? 0);

            if (vaccination == null || vaccination.Patient.Account != ActionContext.CurrentAccount)
            {
                return RedirectToAction("List");
            }

            //if (incident.AssessmentCompleted == true && ActionContext.CurrentUser.HasPermission(Domain.Enumerations.KnownPermision.EditResolvedIncidents) == false)
            //{
            //    return RedirectToAction("View", new { id = id, returnUrl = returnUrl });
            //}

            var formModel = ModelMapper.MapForUpdate<VaccineForm>(vaccination);

            // Since we are using a read-only mapping for patient info we have to manually map this property
            ModelMapper.ReadFromDomain(vaccination.Patient, formModel.Patient);

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(VaccineForm formModel, bool formCancelled, string returnUrl)
        {
            this.ViewData["ExportPath"] = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri.Replace("Edit", "View");
            this.ViewData["IsExportable"] = true;

            try
            {
                if (formCancelled != true)
                {
                    var vaccination = VaccineRepository.Get(formModel.VaccineEntryId ?? 0);

                    if (vaccination != null)
                    {
                        ModelMapper.MapForUpdate(formModel, vaccination);
                        AuditWorker.AuditOnUpdate(vaccination);
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
            var vaccination = VaccineRepository.Get(id ?? 0);

            // TODO var formModel =  new IQI.Intuition.Web.Models.Reporting.Incident.Facility.LineListingIncidentView.IncidentRow(vaccination);
            //return View(formModel);
            return null;
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
            var vaccination = VaccineRepository.Get(id);
            vaccination.Deleted = true;

            return RedirectToAction("Detail", "Patient", new { id = vaccination.Patient.Guid });

        }
    }
}
