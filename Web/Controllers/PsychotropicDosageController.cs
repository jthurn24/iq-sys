using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.PsychotropicDosageChange;
using IQI.Intuition.Domain;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Domain.Services.Psychotropic;
using RedArrow.Framework.ObjectModel.AuditTracking;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount, EnableAccountRestriction]
    public class PsychotropicDosageController : Controller
    {
        public PsychotropicDosageController(
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
            var gridModel = new AdministrationPsychotropicDosageGrid(dataUrl);

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

            var query = PsychotropicRespository.FindDosageChange(
                domain,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            var gridModel = new AdministrationPsychotropicDosageGrid();
            ModelMapper.MapForReadOnly(query, gridModel);

            return gridModel.GetJsonResult();
        }


        [HttpGet]
        public ActionResult Add(int id)
        {

            var admin = PsychotropicRespository.GetAdministration(id);

            if (admin == null || admin.Patient.Account.Id != ActionContext.CurrentAccount.Id)
            {
                throw new Exception("Unauthorized Access");
            }

            var formModel = ModelMapper.MapForCreate<PsychotropicDosageChangeFormAdd>();
            return View(formModel);

        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(PsychotropicDosageChangeFormAdd formModel, int id, bool formCancelled)
        {
            var admin = PsychotropicRespository.GetAdministration(id);

            if (admin == null || admin.Patient.Account.Id != ActionContext.CurrentAccount.Id)
            {
                throw new Exception("Unauthorized Access");
            }

            try
            {
                if (formCancelled != true)
                {

                    var domain = new PsychotropicDosageChange();
                    domain.Administration = admin;
                    admin.DosageChanges.Add(domain);

                    ModelMapper.MapForCreate(formModel, domain);
                    PsychotropicRespository.Add(domain);

                    admin.EvaluateActive();

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
            var domain = PsychotropicRespository.GetDosageChange(id);
            domain.Administration.DosageChanges.Remove(domain);
            AuditWorker.AuditOnUpdate(domain.Administration);
            PsychotropicRespository.Delete(domain);
            domain.Administration.EvaluateActive();

            return RedirectToAction("View", new { controller = "PsychotropicAdministration", id = domain.Administration.Id });
        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
        {
            var domain = PsychotropicRespository.GetDosageChange(id ?? 0);

            if (domain == null || domain.Administration.Patient.Account.Id != ActionContext.CurrentAccount.Id)
            {
                throw new Exception("Unauthorized Access");
            }


            var formModel = ModelMapper.MapForUpdate<PsychotropicDosageChangeFormEdit>(domain);

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(PsychotropicDosageChangeFormEdit formModel, bool formCancelled, int? id, string returnUrl)
        {
            var domain = PsychotropicRespository.GetDosageChange(id ?? 0);

            
            if (domain == null || domain.Administration.Patient.Account.Id != ActionContext.CurrentAccount.Id)
            {
                throw new Exception("Unauthorized Access");
            }

            try
            {
                if (formCancelled != true)
                {
                    
                    if (domain != null)
                    {
                        ModelMapper.MapForUpdate(formModel, domain);

                        var admin = PsychotropicRespository.GetAdministration(domain.Administration.Id);
                        admin.EvaluateActive();
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

        public ActionResult DosageForm(int? frequencyId, int? changeId, string prefix)
        {
            var model = new DosageForm();
            model.Prefix = prefix;

            if (frequencyId.HasValue == false)
            {
                model.Entries = new List<DosageSegmentEntry>();
                return PartialView(model);
            }

            var frequency = PsychotropicRespository.AllFrequencies.Where(x => x.Id == frequencyId).FirstOrDefault();

            IEnumerable<DosageSegment> segments;

            if (changeId.HasValue == false)
            {
                segments = frequency.GetFrequencyDefinition().GetDefaultSegments();
            }
            else
            {
                var change = PsychotropicRespository.GetDosageChange(changeId.Value);

                if (frequencyId.Value != change.Frequency.Id)
                {
                    segments = frequency.GetFrequencyDefinition().GetDefaultSegments();
                }
                else
                {
                    segments = change.Frequency.GetFrequencyDefinition().ReadSegments(change.DosageSegments);
                }
                
            }

            model.Entries = segments.Select(x => new DosageSegmentEntry()
            {
                Description = x.Description,
                ID = x.ID,
                Dosage = x.Dosage,
                Label = x.Label,
                DescriptionOptions = x.DescriptionOptions
            });

            return PartialView(model);
        }
    }
}
