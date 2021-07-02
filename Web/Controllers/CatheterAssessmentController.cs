using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.CatheterAssessment;
using IQI.Intuition.Domain;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Reporting.Graphics;
using System.Drawing;
using RedArrow.Framework.ObjectModel.AuditTracking;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount, EnableAccountRestriction]
    public class CatheterAssessmentController : Controller
    {
        public CatheterAssessmentController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            IPatientRepository patientRepository,
            ICatheterRepository catheterRepository,
            AuditTrackingWorker auditWorker)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            CatheterRepository = catheterRepository.ThrowIfNullArgument("catheterRepository");
            AuditWorker = auditWorker;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual ICatheterRepository CatheterRepository { get; private set; }
        protected virtual AuditTrackingWorker AuditWorker { get; private set; }

        [HttpGet]
        public ActionResult EntryCatheterAssessmentList(int id)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("EntryCatheterAssessmentListData", new { id = id });
            var gridModel = new EntryAssessmentGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult EntryCatheterAssessmentListData(AssessmentGridRequest requestModel, int id)
        {
            var entry = CatheterRepository.Get(id);

            var query = CatheterRepository.FindAssessment(
                entry,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            var gridModel = new EntryAssessmentGrid();
            ModelMapper.MapForReadOnly(query, gridModel);

            return gridModel.GetJsonResult();

        }


        [HttpGet]
        public ActionResult Add(int id)
        {
            var entry = CatheterRepository.Get(id);

            AssessmentForm formModel;
            formModel = ModelMapper.MapForCreate<AssessmentForm>();

            formModel.Id = null;
            formModel.AssessmentDate = DateTime.Today;
            formModel.Floor = entry.Patient.Room.Wing.Floor.Id;
            formModel.Wing = entry.Patient.Room.Wing.Id;
            formModel.Room = entry.Patient.Room.Id;

            return View("Edit", formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(AssessmentForm formModel, int id, bool formCancelled)
        {
            var report = CatheterRepository.Get(id);

            try
            {
                if (formCancelled != true)
                {
                    var domain = new CatheterAssessment();
                    ModelMapper.MapForCreate(formModel, domain);
                    domain.CatheterEntry = report;
                    domain.Patient = report.Patient;
                    CatheterRepository.Add(domain);
                    report.Assessments.Add(domain);
                    EvaluateAction(domain);
                    AuditWorker.AuditOnUpdate(report);

                    if (report.Room == null)
                    {
                        report.Room = domain.Room;
                    }
                }

                return RedirectToAction("View", new { controller = "Catheter", id = id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            formModel.ClientData = new AssessmentFormClientData();
            ModelMapper.AssignDefaults(formModel.ClientData);

            return View("Edit", formModel);
        }

        public ActionResult Remove(int id)
        {
            var assessment = CatheterRepository.GetAssessment(id);
            assessment.CatheterEntry.Assessments.Remove(assessment);
            CatheterRepository.Delete(assessment);
            AuditWorker.AuditOnUpdate(assessment.CatheterEntry);

            return RedirectToAction("View", new { controller = "Catheter", id = assessment.CatheterEntry.Id });
        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
        {
            var domain = CatheterRepository.GetAssessment(id ?? 0);
            var formModel = ModelMapper.MapForUpdate<AssessmentForm>(domain);
            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(AssessmentForm formModel, bool formCancelled, int? id)
        {
            var domain = CatheterRepository.GetAssessment(id.Value);

            try
            {
                if (formCancelled != true)
                {
                    if (domain != null)
                    {
                        ModelMapper.MapForUpdate(formModel, domain);
                        EvaluateAction(domain);
                        AuditWorker.AuditOnUpdate(domain.CatheterEntry);

                    }
                }

                return RedirectToAction("View", new { controller = "Catheter", id = domain.CatheterEntry.Id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            formModel.ClientData = new AssessmentFormClientData();
            ModelMapper.AssignDefaults(formModel.ClientData);

            return View(formModel);
        }


        private void EvaluateAction(Domain.Models.CatheterAssessment assessment)
        {
            if (assessment.Action == (int)Domain.Enumerations.CatheterAction.Attempt
                && assessment.RemovedAndReplaced == false
                && assessment.CatheterEntry.DiscontinuedOn.HasValue == false)
            {
                assessment.CatheterEntry.DiscontinuedOn = assessment.AssessmentDate;
            }

        }

    }
}
