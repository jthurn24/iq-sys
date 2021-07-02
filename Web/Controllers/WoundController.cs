using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Wound;
using IQI.Intuition.Domain;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Reporting.Graphics;
using System.Drawing;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount, EnableAccountRestriction]
    public class WoundController : Controller
    {
        public WoundController(
            IActionContext actionContext, 
            IModelMapper modelMapper, 
            IPatientRepository patientRepository,
            IWoundRepository woundRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            PatientRepository = patientRepository.ThrowIfNullArgument("patientRepository");
            WoundRepository = woundRepository.ThrowIfNullArgument("WoundRepository");
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IPatientRepository PatientRepository { get; private set; }
        protected virtual IWoundRepository WoundRepository { get; private set; }

        [HttpGet]
        public ActionResult List()
        {
            string dataUrl = Url.Action("ListData");
            var gridModel = new WoundGrid(dataUrl);

            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(WoundGridRequest requestModel)
        {
            var query = WoundRepository.FindReport(
                ActionContext.CurrentFacility,
                requestModel.PatientFullName,
                requestModel.RoomAndWingName,
                requestModel.FirstNoted,
                requestModel.ResolvedOn,
                true,
                requestModel.StageName,
                requestModel.SiteName,
                requestModel.TypeName,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            var gridModel = new WoundGrid();
            ModelMapper.MapForReadOnly(query, gridModel);

            return gridModel.GetJsonResult();
        }


        [HttpGet]
        public ActionResult PatientWoundList(Guid? id)
        {
            string dataUrl = Url.Action("PatientWoundListData", new { PatientGuid = id });
            var gridModel = new PatientWoundGrid(dataUrl);

            return PartialView(gridModel);
        }

        [HttpGet]
        public ActionResult PatientWoundListData(WoundGridRequest requestModel)
        {
            var patient = ActionContext.CurrentFacility.FindPatient(requestModel.PatientGuid);

            var query = WoundRepository.Find(
                patient,
                requestModel.SortBy,
                requestModel.SortDescending,
                requestModel.PageNumber,
                requestModel.PageSize);

            var gridModel = new PatientWoundGrid();
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
                    var formModel = ModelMapper.MapForCreate<WoundForm>();
                    ModelMapper.ReadFromDomain(patient, formModel.Patient);

                    formModel.LoadSiteQuickList(WoundRepository);

                    return View("Edit",formModel);
                }
            }

            return RedirectToAction("Detail", new { controller = "Patient", id = id });
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Add(WoundForm formModel, Guid? id, bool formCancelled)
        {
            Patient patient = null;



            try
            {
                if (formCancelled != true)
                {
                    patient = ActionContext.CurrentFacility.FindPatient(id.Value);
                    var domain = new WoundReport(patient);
                    ModelMapper.MapForCreate(formModel, domain);
                    WoundRepository.Add(domain);
                    return RedirectToAction("Add", new { controller = "WoundAssessment", id = domain.Id });
                }

                return RedirectToAction("Detail", new { controller = "Patient", id = id });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            formModel.LoadSiteQuickList(WoundRepository);
            return View("Edit",formModel);
        }

        public ActionResult Remove(int id)
        {
            var domain = WoundRepository.GetReport(id);
            domain.Deleted = true;

            return RedirectToAction("Detail", new { controller = "Patient", id = domain.Patient.Guid });

        }

        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
        {
            var domain = WoundRepository.GetReport(id ?? 0);
            var formModel = ModelMapper.MapForUpdate<WoundForm>(domain);
            ModelMapper.ReadFromDomain(domain.Patient, formModel.Patient);

            formModel.LoadSiteQuickList(WoundRepository);

            return View(formModel);
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(WoundForm formModel, bool formCancelled, int? id, string returnUrl)
        {
            var domain = WoundRepository.GetReport(id.Value);

            try
            {
                if (formCancelled != true)
                {
                    if (domain != null)
                    {
                        ModelMapper.MapForUpdate(formModel, domain);
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

            ModelMapper.ReadFromDomain(domain.Patient, formModel.Patient);
            formModel.LoadSiteQuickList(WoundRepository);

            return View(formModel);
        }

        public ActionResult View(int? id)
        {
            var domain = WoundRepository.GetReport(id ?? 0);
            var formModel = ModelMapper.MapForUpdate<WoundInfo>(domain);
            return View(formModel);
        }

        public ActionResult BodyImage(int? x, int? y, bool? disabled, int? width, int? height)
        {
            var chart = new BodyChart(disabled.ToBooleanSafely());

            if (x.HasValue && y.HasValue)
            {
                chart.Circles.Add(new BodyChart.Circle() { Coordinates = string.Concat(x, ",", y), ShadingOpacity = 255, ShadingColor = Color.Red, Width = 20 });
            }

            var stream = BodyChart.GenerateImage(chart.SerializeForRender(), Server.MapPath("/Content/images/body.bmp"),width,height);
            return File(stream, "image/jpeg");
        }

        public ActionResult GetSite(double x, double y)
        {
            var sites = WoundRepository.AllSites;

            foreach (var site in sites.OrderByDescending(xx => xx.Priority))
            {
                if (site.TopLeftX <= x && site.TopLeftY <= y && site.BottomRightX >= x && site.BottomRightY >= y)
                {
                    return  Json(new { id = site.Id, name = site.Name }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { id = string.Empty, name = "Unknown" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetFacilityPressureUlcerOverview()
        {
            var model = new ActiveWoundMap();
            model.Graph = new BodyGraph();
            model.Sites = new List<WoundSite>();

            var activeWounds = WoundRepository.FindActive(ActionContext.CurrentFacility);
            activeWounds = activeWounds.Where(x => x.WoundType.Id == (int)Domain.Enumerations.KnownWoundType.PressureUlcer);

            foreach (var site in WoundRepository.AllSites)
            {
                if (activeWounds.Where(x => x.Site.Id == site.Id).Count() > 0)
                {
                    model.Graph.Areas.Add(new BodyGraph.Area()
                    {
                        TopLeftX = site.TopLeftX.Value,
                        TopLeftY = site.TopLeftY.Value,
                        BottomRightX = site.BottomRightX.Value,
                        BottomRightY = site.BottomRightY.Value,
                        ShadingColor = Color.Blue,
                        ShadingOpacity = 60
                    });

                    model.Sites.Add(site);

                }
            }


            return PartialView(model);
        }

        public ActionResult GetFacilityPressureUlcerDetails(int siteId)
        {
            var model = new ActiveWoundMapDetail();
            model.Wounds = new List<ActiveWoundMapDetail.Wound>();

            var activeWounds = WoundRepository.FindActive(ActionContext.CurrentFacility);
            activeWounds = activeWounds.Where(x => x.WoundType.Id == (int)Domain.Enumerations.KnownWoundType.PressureUlcer);
            activeWounds = activeWounds.Where(x => x.Site.Id == siteId);

            var site = WoundRepository.AllSites.Where(x => x.Id == siteId).First();

            model.SiteName = site.Name;

            foreach (var w in activeWounds)
            {
                var e = new ActiveWoundMapDetail.Wound();
                e.PatientGuid = w.Patient.Guid.ToString();
                e.PatientName = w.Patient.FullName;
                e.Stage = w.CurrentStage != null ? w.CurrentStage.Name : "N/A";
                model.Wounds.Add(e);
            }



            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
