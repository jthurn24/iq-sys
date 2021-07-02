using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.PsychotropicAdministrationPRN;
using IQI.Intuition.Domain;
using IQI.Intuition.Web.Attributes;
using RedArrow.Framework.ObjectModel.AuditTracking;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount, EnableAccountRestriction]
    public class PsychotropicAdministrationPRNController : Controller
    {
        public PsychotropicAdministrationPRNController(
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
        public ActionResult AdministrationList(int id)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("AdministrationListData", new { AdministrationId = id });
            var gridModel = new AdministrationPRNGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult AdministrationListData(AdministrationPsychotropicDosageGridRequest requestModel)
        {
            var domain = PsychotropicRespository.GetAdministration(requestModel.AdministrationId.Value);

            if (domain == null || domain.Patient.Account.Id != ActionContext.CurrentAccount.Id)
            {
                return null;
            }

            var query = PsychotropicRespository.FindPRN(
                domain,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            var gridModel = new AdministrationPRNGrid();
            ModelMapper.MapForReadOnly(query, gridModel);

            return gridModel.GetJsonResult();
        }


        [HttpGet]
        public ActionResult Add(int id)
        {

            var admin = PsychotropicRespository.GetAdministration(id);
            var formModel = ModelMapper.MapForCreate<PsychotropicAdministrationPRNFormEdit>();
            formModel.GivenOn = DateTime.Today;
            return View(formModel);

        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(PsychotropicAdministrationPRNFormEdit formModel, int id, bool formCancelled)
        {
            var admin = PsychotropicRespository.GetAdministration(id);

            try
            {
                if (formCancelled != true)
                {

                    var domain = new PsychotropicAdministrationPRN();
 
                    ModelMapper.MapForCreate(formModel, domain);

                    domain.Administration = admin;
                    admin.PRNs.Add(domain);
                    PsychotropicRespository.Add(domain);

                    AuditWorker.AuditOnUpdate(admin);
                }

                return RedirectToAction("View", new { controller = "PsychotropicAdministration", id = id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            return View(formModel);
        }

        public ActionResult Remove(int id)
        {
            var domain = PsychotropicRespository.GetPRN(id);

            AuditWorker.AuditOnUpdate(domain.Administration);

            PsychotropicRespository.Delete(domain);
            return RedirectToAction("View", new { controller = "PsychotropicAdministration", id = domain.Administration.Id });
        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
        {
            var domain = PsychotropicRespository.GetPRN(id ?? 0);
            var formModel = ModelMapper.MapForUpdate<PsychotropicAdministrationPRNFormEdit>(domain);
            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(PsychotropicAdministrationPRNFormEdit formModel, bool formCancelled, int? id, string returnUrl)
        {
            var domain = PsychotropicRespository.GetPRN(id ?? 0);

            try
            {
                if (formCancelled != true)
                {
                    
                    if (domain != null)
                    {
                        ModelMapper.MapForUpdate(formModel, domain);
                        var admin = PsychotropicRespository.GetAdministration(domain.Administration.Id);
                        AuditWorker.AuditOnUpdate(admin);
                    }
                }

                if (returnUrl.IsNotNullOrWhiteSpace())
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("View", new { controller = "PsychotropicAdministration", id = domain.Administration.Id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            return View(formModel);
        }
    }
}
