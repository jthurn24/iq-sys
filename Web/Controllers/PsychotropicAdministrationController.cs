using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Services.Psychotropic;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.PsychotropicAdministration;
using IQI.Intuition.Domain;
using IQI.Intuition.Web.Attributes;


namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount, EnableAccountRestriction]
    public class PsychotropicAdministrationController : Controller
    {
        public PsychotropicAdministrationController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IPatientRepository patientRepository,
            IPsychotropicRespository psychotropicRespository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            PsychotropicRespository = psychotropicRespository.ThrowIfNullArgument("psychotropicRespository");
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual IPsychotropicRespository PsychotropicRespository { get; private set; }


        [HttpGet]
        public ActionResult List()
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("ListData");
            var gridModel = new PsychotropicAdministrationGrid(dataUrl);

            // By convention, we normally use a dedicated model class for each view but, since this view 
            //  does not require anything more than the grid itself, we are using the grid model instead
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(PsychotropicAdministrationGridRequest requestModel)
        {
            var query = PsychotropicRespository.FindAdministration(
                ActionContext.CurrentFacility,
                requestModel.Name,
                requestModel.PatientFullName,
                requestModel.PatientRoomWingName,
                requestModel.DrugTypeName,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new PsychotropicAdministrationGrid();
            ModelMapper.MapForReadOnly(query, gridModel);

            return gridModel.GetJsonResult();
        }

        [HttpGet]
        public ActionResult PatientPsychotropicAdministrationList(Guid? id)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("PatientPsychotropicAdministrationListData", new { PatientGuid = id });
            var gridModel = new PatientPsychotropicAdministrationGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult PatientPsychotropicAdministrationListData(PsychotropicAdministrationGridRequest requestModel)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(requestModel.PatientGuid ?? Guid.Empty);

            if (patient == null)
            {
                return null;
            }

            var query = PsychotropicRespository.FindAdministration(
                patient,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            var gridModel = new PatientPsychotropicAdministrationGrid();
            ModelMapper.MapForReadOnly(query, gridModel);

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
                    var formModel = ModelMapper.MapForCreate<PsychotropicAdministrationFormAdd>();
                    return View(formModel);
                }
            }

            return RedirectToAction("Detail", new { controller = "Patient", id = id });
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(PsychotropicAdministrationFormAdd formModel, Guid? id, bool formCancelled)
        {
            Patient patient = null;

            try
            {
                if (formCancelled != true)
                {



                    patient = ActionContext.CurrentFacility.FindPatient(id.Value);

                    if (patient == null)
                    {
                        return RedirectToAction("List");
                    }

                    var domain = new PsychotropicAdministration(patient);

                    ModelMapper.MapForCreate(formModel, domain);
                    PsychotropicRespository.Add(domain);
                    PsychotropicRespository.Add(domain.DosageChanges.First());

                    domain.EvaluateActive();

                }

                return RedirectToAction("Detail", new { controller = "Patient", id = id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            return View(formModel);
        }

        public ActionResult Remove(int id)
        {
            var domain = PsychotropicRespository.GetAdministration(id);
            domain.Deleted = true;
            return RedirectToAction("Detail", new { controller = "Patient", id = domain.Patient.Guid });

        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
        {
            var domain = PsychotropicRespository.GetAdministration(id ?? 0);
            var formModel = ModelMapper.MapForUpdate<PsychotropicAdministrationFormEdit>(domain);
            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(PsychotropicAdministrationFormEdit formModel, bool formCancelled, int? id, string returnUrl)
        {

            try
            {
                if (formCancelled != true)
                {
                    var domain = PsychotropicRespository.GetAdministration(id.Value);

                    if (domain != null)
                    {
                        ModelMapper.MapForUpdate(formModel, domain);
                        domain.EvaluateActive();
                    }
                }

                if (returnUrl.IsNotNullOrWhiteSpace())
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("View", new { id = id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            return View(formModel);
        }

        public ActionResult View(int? id)
        {
            var domain = PsychotropicRespository.GetAdministration(id ?? 0);
            var formModel = ModelMapper.MapForUpdate<PsychotropicAdministrationInfo>(domain);
            return View(formModel);
        }
    }
}
