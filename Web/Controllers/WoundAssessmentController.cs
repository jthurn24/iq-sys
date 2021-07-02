using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.WoundAssessment;
using IQI.Intuition.Domain;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Reporting.Graphics;
using System.Drawing;
using RedArrow.Framework.ObjectModel.AuditTracking;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount, EnableAccountRestriction]
    public class WoundAssessmentController : Controller
    {
        public WoundAssessmentController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IPatientRepository patientRepository,
            IWoundRepository woundRepository,
            AuditTrackingWorker auditWorker)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            WoundRepository = woundRepository.ThrowIfNullArgument("WoundRepository");
            AuditWorker = auditWorker;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual IWoundRepository WoundRepository { get; private set; }
        protected virtual AuditTrackingWorker AuditWorker { get; private set; }

        [HttpGet]
        public ActionResult ReportWoundAssessmentList(int id)
        {
            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("ReportWoundAssessmentListData", new { id = id });
            var gridModel = new ReportAssessmentGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult ReportWoundAssessmentListData(AssessmentGridRequest requestModel, int id)
        {
            var report = WoundRepository.GetReport(id);

            var query = WoundRepository.FindAssessment(
                report,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            var gridModel = new ReportAssessmentGrid();
            ModelMapper.MapForReadOnly(query, gridModel);

            return gridModel.GetJsonResult();

        }


        [HttpGet]
        public ActionResult Add(int id)
        {
            var report = WoundRepository.GetReport(id);
            var lastAssessment = report.Assessments.OrderBy(x => x.AssessmentDate).LastOrDefault();

            AssessmentForm formModel;


            if (lastAssessment != null)
            {
                formModel = ModelMapper.MapForUpdate<AssessmentForm>(lastAssessment);
                formModel.Progress = Enumerations.WoundProgress.NoChange;
            }
            else
            {
                formModel = ModelMapper.MapForCreate<AssessmentForm>();
                formModel.Progress = Enumerations.WoundProgress.New;
            }

            if(report.WoundType.Id != (int)Domain.Enumerations.KnownWoundType.PressureUlcer)
            {
                formModel.StageLocked = true;
                formModel.Stage = (int)Domain.Enumerations.KnownWoundStage.NotApplicable;
            }


            formModel.AssessmentDate = DateTime.Today;
            formModel.Floor = report.Patient.Room.Wing.Floor.Id;
            formModel.Wing = report.Patient.Room.Wing.Id;
            formModel.Room = report.Patient.Room.Id;

            return View("Edit", formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(AssessmentForm formModel, int id, bool formCancelled)
        {
            var report = WoundRepository.GetReport(id);

            try
            {
                if (formCancelled != true)
                {
                    var domain = new WoundAssessment();
                    ModelMapper.MapForCreate(formModel, domain);
                    domain.Report = report;
                    WoundRepository.Add(domain);
                    report.Assessments.Add(domain);
                    report.EvaluateCurrentStage();
                    AuditWorker.AuditOnUpdate(report);

                    if (report.Room == null)
                    {
                        report.Room = domain.Room;
                    }
                }

                return RedirectToAction("View", new { controller = "Wound", id = id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            formModel.ClientData = new AssessmentFormClientData();
            ModelMapper.AssignDefaults(formModel.ClientData);

            return View("Edit",formModel);
        }

        public ActionResult Remove(int id)
        {
            var domain = WoundRepository.GetAssessment(id);
            domain.Report.Assessments.Remove(domain);
            WoundRepository.Delete(domain);
            domain.Report.EvaluateCurrentStage();
            AuditWorker.AuditOnUpdate(domain.Report);

            return RedirectToAction("View", new { controller = "Wound", id = domain.Report.Id });

        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
        {
            var domain = WoundRepository.GetAssessment(id ?? 0);
            var formModel = ModelMapper.MapForUpdate<AssessmentForm>(domain);

            if (domain.Report.WoundType.Id != (int)Domain.Enumerations.KnownWoundType.PressureUlcer)
            {
                formModel.StageLocked = true;
                formModel.Stage = (int)Domain.Enumerations.KnownWoundStage.NotApplicable;
            }

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(AssessmentForm formModel, bool formCancelled, int? id)
        {
            var domain = WoundRepository.GetAssessment(id.Value);

            try
            {
                if (formCancelled != true)
                {
                    if (domain != null)
                    {
                        ModelMapper.MapForUpdate(formModel, domain);
                        domain.Report.EvaluateCurrentStage();
                        AuditWorker.AuditOnUpdate(domain.Report);
                    }
                }

                return RedirectToAction("View", new { controller = "Wound", id = domain.Report.Id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            formModel.ClientData = new AssessmentFormClientData();
            ModelMapper.AssignDefaults(formModel.ClientData);

            return View(formModel);
        }


    }
}
