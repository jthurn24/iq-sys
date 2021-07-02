using System;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using RedArrow.Framework.ObjectModel.AuditTracking;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Catheter;
using IQI.Intuition.Web.Attributes;
using System.Linq;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount, EnableAccountRestriction]
    public class CatheterController : Controller
    {
        public CatheterController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            ICatheterRepository catheterRepository,
            AuditTrackingWorker auditWorker
            )
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            CatheterRepository = catheterRepository.ThrowIfNullArgument("CatheterRepository");
            AuditWorker = auditWorker;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual ICatheterRepository CatheterRepository { get; private set; }

        protected virtual AuditTrackingWorker AuditWorker { get; private set; }

        [HttpGet]
        public ActionResult List()
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("ListData");
            var gridModel = new CatheterGrid(dataUrl);

            // By convention, we normally use a dedicated model class for each view but, since this view 
            //  does not require anything more than the grid itself, we are using the grid model instead
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(CatheterGridRequest requestModel)
        {
            var CatheterQuery = CatheterRepository.Find(
                ActionContext.CurrentFacility,
                requestModel.PatientFullName,
                requestModel.RoomAndWingName,
                requestModel.StartedOn,
                requestModel.DiscontinuedOn,
                requestModel.Diagnosis,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new CatheterGrid();
            ModelMapper.MapForReadOnly(CatheterQuery, gridModel);

            return gridModel.GetJsonResult();
        }

        [HttpGet]
        public ActionResult PatientCatheterList(Guid? id)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("PatientCatheterListData", new { PatientGuid = id });
            var gridModel = new PatientCatheterGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult PatientCatheterListData(CatheterGridRequest requestModel)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(requestModel.PatientGuid ?? Guid.Empty);

            if (patient == null)
            {
                return null;
            }

            var CatheterQuery = CatheterRepository.Find(
                patient,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new PatientCatheterGrid();
            ModelMapper.MapForReadOnly(CatheterQuery, gridModel);

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
                    var formModel = ModelMapper.MapForCreate<CatheterForm>();
                    formModel.Room = patient.Room.Id;
                    formModel.Wing = patient.Room.Wing.Id;
                    formModel.Floor = patient.Room.Wing.Floor.Id;
                    formModel.StartDate = DateTime.Today;

                    // Since we are using a read-only mapping for patient info we have to manually map this property
                    ModelMapper.ReadFromDomain(patient, formModel.Patient);

                    return View("Edit", formModel);
                }
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Add(CatheterForm formModel, bool? cancel)
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

                    var catheter = new CatheterEntry(patient);
                    ModelMapper.MapForCreate(formModel, catheter);
                    CatheterRepository.Add(catheter);
                    return RedirectToAction("Add", new { controller = "CatheterAssessment", id = catheter.Id });
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
            var Catheter = CatheterRepository.Get(id ?? 0);

            if (Catheter == null || Catheter.Patient.Account != ActionContext.CurrentAccount)
            {
                return RedirectToAction("List");
            }

            var formModel = ModelMapper.MapForUpdate<CatheterForm>(Catheter);

            // Since we are using a read-only mapping for patient info we have to manually map this property
            ModelMapper.ReadFromDomain(Catheter.Patient, formModel.Patient);

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(CatheterForm formModel, bool formCancelled, string returnUrl)
        {
            try
            {
                if (formCancelled != true)
                {
                    var Catheter = CatheterRepository.Get(formModel.CatheterEntryId ?? 0);

                    if (Catheter != null)
                    {
                        ModelMapper.MapForUpdate(formModel, Catheter);
                        AuditWorker.AuditOnUpdate(Catheter);
                    }
                }

                if (returnUrl.IsNotNullOrWhiteSpace())
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("View", new { id = formModel.CatheterEntryId ?? 0 });
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
            var catheter = CatheterRepository.Get(id ?? 0);
            var formModel = ModelMapper.MapForUpdate<CatheterInfo>(catheter);
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
            var Catheter = CatheterRepository.Get(id);
            Catheter.Deleted = true;

            return RedirectToAction("Detail", "Patient", new { id = Catheter.Patient.Guid });

        }

    }
}