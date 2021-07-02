using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Patient;
using IQI.Intuition.Domain;
using IQI.Intuition.Web.Attributes;
using RedArrow.Framework.ObjectModel.AuditTracking;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class PatientController : Controller
    {
        public PatientController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IPatientRepository patientRepository,
            IPsychotropicRespository psychotropicRespository,
            AuditTrackingWorker auditWorker)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            PsychotropicRespository = psychotropicRespository.ThrowIfNullArgument("psychotropicRespository");
            AuditWorker = auditWorker;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual IPsychotropicRespository PsychotropicRespository { get; private set; }
        protected virtual AuditTrackingWorker AuditWorker { get; private set; }

        [HttpGet]
        public ActionResult List(bool? showAdmittedOnly)
        {
            // Here we are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("ListData", new { showAdmittedOnly = (showAdmittedOnly.HasValue ? showAdmittedOnly.Value : true) });
            var gridModel = new PatientGrid(dataUrl);

            // By convention, we normally use a dedicated model class for each view but, since this view 
            //  does not require anything more than the grid itself, we are using the grid model instead
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(PatientGridRequest requestModel, bool showAdmittedOnly)
        {
            if (showAdmittedOnly)
            {
                requestModel.Status = Enumerations.PatientStatus.Admitted;
            }
            else
            {
                requestModel.Status =  null ;
            }

            var patientQuery = PatientRepository.Find(
                ActionContext.CurrentFacility,
                requestModel.FirstName,
                requestModel.LastName,
                requestModel.BirthDate,
                requestModel.RoomWingName,
                requestModel.RoomName, 
                requestModel.Status, 
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber, 
                requestModel.PageSize);

            // Here we are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new PatientGrid(patient => Url.Action("Detail", new { id = patient.Guid }));
            ModelMapper.MapForReadOnly(patientQuery, gridModel);

            var result = gridModel.GetJsonResult();

            return result;
        }

        [HttpGet]
        public ActionResult Detail(Guid? id)
        {
            var patient = PatientRepository.Get(id ?? Guid.Empty);

            if (patient == null || patient.Account != ActionContext.CurrentAccount)
            {
                return RedirectToAction("List");
            }

            var viewModel = ModelMapper.MapForReadOnly<Chart>(patient);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Add()
        {
            var formModel = ModelMapper.MapForCreate<PatientForm>();
            formModel.RoomChangeAt = DateTime.Today;
            formModel.StatusChangedAt = DateTime.Today;

            if (ActionContext.CurrentFacility.Floors.Count() == 1)
            {
                var floor = ActionContext.CurrentFacility.Floors.FirstOrDefault();
                formModel.Floor = floor.Id ;

                if (floor.Wings.Count() == 1)
                {
                    var wing = floor.Wings.FirstOrDefault();
                    formModel.Wing = wing.Id;
                }
            }

            return View("Edit", formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(PatientForm formModel, bool formCancelled)
        {
            try
            {
                if (formCancelled)
                {
                    return RedirectToAction("List");
                }

                var patient = new Patient(ActionContext.CurrentAccount);
                ModelMapper.MapForCreate(formModel, patient);
                BindStatus(formModel, patient);
                BindRoom(formModel, patient);
                PatientRepository.Add(patient);

                return RedirectToAction("Detail", new { id = patient.Guid });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            // Need to re-map the select list options since they are not passed back to the server
            ModelMapper.AssignDefaults(formModel.ClientData);

            return View("Edit", formModel);
        }

        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            var patient = PatientRepository.Get(id ?? Guid.Empty);

            if (patient == null || patient.Account != ActionContext.CurrentAccount)
            {
                return RedirectToAction("List");
            }

            var formModel = ModelMapper.MapForUpdate<PatientForm>(patient);
            formModel.RoomChangeAt = DateTime.Today;
            formModel.StatusChangedAt = DateTime.Today;

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(PatientForm formModel, bool formCancelled)
        {
            try
            {
                if (formCancelled != true)
                {
                    var patient = PatientRepository.Get(formModel.Guid ?? Guid.Empty);

                    if (patient != null && patient.Account == ActionContext.CurrentAccount)
                    {
                        ModelMapper.MapForUpdate(formModel, patient);
                        AuditWorker.AuditOnUpdate(patient);
                        BindStatus(formModel, patient);
                        BindRoom(formModel, patient);
                    }
                }

                return RedirectToAction("Detail", new { id = formModel.Guid });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            // Need to re-map the select list options since they are not passed back to the server
            ModelMapper.AssignDefaults(formModel.ClientData);
            

            return View(formModel);
        }
        
        public ActionResult QuickSearch(string term)
        {
            if (term.IsNullOrWhiteSpace())
            {
                return null;
            }

            var result = PatientRepository
                .FindByName(ActionContext.CurrentFacility, term, 20)
                .Select(patient => new 
                {
                    label = patient.GetLastName() + ", " + patient.GetFirstName(),
                    value = term,
                    id = patient.Guid
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void BindStatus(PatientForm formModel, Patient entity)
        {
            if (formModel.CurrentStatus != formModel.NewStatus || formModel.CurrentStatus == null)
            {
                if (formModel.StatusChangedAt.HasValue == false)
                {
                    throw new ModelMappingException(new ValidationError[] { new ValidationError("StatusChangedAt", "You must supply a valid status change date") } );
                }

                var statusChange = new PatientStatusChange();
                statusChange.Status = (Enumerations.PatientStatus)formModel.NewStatus;
                statusChange.StatusChangedAt = formModel.StatusChangedAt.Value;
                statusChange.Patient = entity;
                entity.AddStatusChange(statusChange);
                entity.CurrentStatus = (Enumerations.PatientStatus)formModel.NewStatus;

                var service = new Infrastructure.Services.BusinessLogic.PatientEvents.StatusChange(PsychotropicRespository);
                service.Handle(entity,
                    (Enumerations.PatientStatus)formModel.NewStatus,
                    formModel.StatusChangedAt.Value);

            }

        }

        private void BindRoom(PatientForm formModel, Patient entity)
        {
            if (formModel.Room != formModel.CurrentRoom || formModel.CurrentRoom == null)
            {

                if (formModel.RoomChangeAt.HasValue == false)
                {
                    throw new ModelMappingException(new ValidationError[] { new ValidationError("RoomChangedAt", "You must supply a valid room change date") });
                }

                var roomChange = new PatientRoomChange();
                roomChange.Room = ActionContext.CurrentFacility.Floors
                    .SelectMany(x => x.Wings)
                    .SelectMany(x => x.Rooms)
                    .Where(x => x.Id == formModel.Room)
                    .First();

                roomChange.RoomChangedAt = formModel.RoomChangeAt.Value;

                roomChange.Patient = entity;

                entity.AddRoomChange(roomChange);

            }

        }

        public ActionResult Remove(Guid id)
        {
            var model = new RemovePatient();
            var entity = PatientRepository.Get(id);

            model.Patient = entity.Id;
            model.PatientName = entity.FullName;

            model.DestinationPatientOptions = PatientRepository.Find(ActionContext.CurrentFacility)
                .Where(x => x.Id != entity.Id)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem()
                {
                    Text = string.Concat(x.FullName, " ", x.BirthDate.HasValue ? x.BirthDate.Value.ToString("MM/dd/yy") : string.Empty),
                    Value = x.Id.ToString()
                })
                .Prepend(new SelectListItem() { Text = "Select....", Value = "" });

            return View(model);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Remove(RemovePatient model)
        {

            try
            {

                var errors = new List<ValidationError>();

                if (model.Patient == null)
                {
                    errors.Add(new ValidationError("Patient", "You must select a patient to remove"));
                }

                if (model.MoveData)
                {
                    if (model.DestinationPatient == null)
                    {
                        errors.Add(new ValidationError("DestinationPatient", "You must select a destination patient"));
                    }

                    if (model.DestinationPatient != null && model.DestinationPatient == model.Patient)
                    {
                        errors.Add(new ValidationError("DestinationPatient", "Destination patient can't be the same patient you are removing."));
                    }
                }


                if (errors.Count < 1)
                {
                     
                    var srcPatient = PatientRepository.Get(model.Patient.Value);

                    var auditEntry = new Domain.Models.AuditEntry();
                    auditEntry.AuditType = Enumerations.AuditEntryType.RemovedPatient;


                    var service = new Infrastructure.Services.BusinessLogic.PatientEvents.RemoveRecord();

                    if (model.MoveData)
                    {
                        var destPatient = PatientRepository.Get(model.DestinationPatient.Value);
                        service.Handle(srcPatient, destPatient);
                    }
                    else
                    {
                        service.Handle(srcPatient,null);
                    }

                    srcPatient.Deleted = true;

                }
                else
                {
                    throw new ModelMappingException(errors);
                }

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (ModelMappingException ex)
            {

                model.DestinationPatientOptions = PatientRepository.Find(ActionContext.CurrentFacility)
                    .OrderBy(x => x.FullName)
                    .Select(x => new SelectListItem()
                    {
                        Text = string.Concat(x.FullName, " ", x.BirthDate.HasValue ? x.BirthDate.Value.ToString("MM/dd/yy") : string.Empty),
                        Value = x.Id.ToString()
                    })
                    .Prepend(new SelectListItem() { Text = "Select....", Value = "" });

                ModelState.AddModelMappingErrors(ex);
            }


            return View(model);
        }

        public ActionResult EditStatus(int id)
        {
            var change = PatientRepository.GetStatusChange(id);

            return View(change.StatusChangedAt);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult EditStatus(int id, DateTime changeDate, bool formCancelled)
        {
            var change = PatientRepository.GetStatusChange(id);


            if (formCancelled)
            {
                return RedirectToAction("Detail", "Patient", new { id = change.Patient.Guid });
            }

            change.StatusChangedAt = changeDate;

            return RedirectToAction("Detail", "Patient", new { id = change.Patient.Guid });
        }



        public ActionResult SelectPatient()
        {
            return PartialView();
        }


        public ActionResult SelectPatientSearch(string firstName, string lastName, bool includeDischarged)
        {
            var patients = PatientRepository.Find(ActionContext.CurrentFacility);

            if(!includeDischarged)
            {
                patients = patients.Where(x => x.CurrentStatus == Enumerations.PatientStatus.Admitted);
            }

            if (firstName.IsNotNullOrEmpty())
            {
                patients = patients.Where(x => x.GetFirstName().ToLower().Contains(firstName.ToLower()));
            }

            if (lastName.IsNotNullOrEmpty())
            {
                patients = patients.Where(x => x.GetLastName().ToLower().Contains(lastName.ToLower()));
            }

            var model = patients.Select(
                x => ModelMapper.MapForReadOnly<PatientInfo>(x));

            return PartialView(model);

        }

    }
}
