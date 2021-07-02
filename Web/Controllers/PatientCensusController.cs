using System;
using System.Web.Mvc;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.PatientCensus;
using IQI.Intuition.Web.Attributes;
using RedArrow.Framework.Logging;

namespace IQI.Intuition.Web.Controllers
{
    [RequiresActiveAccount]
    public class PatientCensusController : Controller
    {
        public PatientCensusController(
            IActionContext actionContext,
            IModelMapper modelMapper,
            IPatientCensusRepository patientCensusRepository,
            ILog logger)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            PatientCensusRepository = patientCensusRepository.ThrowIfNullArgument("PatientCensusRepository");
            Logger = logger;
        }

        protected virtual IActionContext ActionContext { get; private set; }

        protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IPatientCensusRepository PatientCensusRepository { get; private set; }

        protected virtual ILog Logger { get; private set; }

        [HttpGet]
        public ActionResult List()
        {
            PatientCensusRepository.Ensure(ActionContext.CurrentFacility, 2);

            // We are setting up the grid for rendering so we pass in the data URL
            string dataUrl = Url.Action("ListData");
            var gridModel = new PatientCensusGrid(dataUrl);

            // By convention, we normally use a dedicated model class for each view but, since this view 
            //  does not require anything more than the grid itself, we are using the grid model instead
            return View(gridModel);
        }

        [HttpGet]
        public ActionResult ListData(PatientCensusGridRequest requestModel)
        {
            var PatientCensusQuery = PatientCensusRepository.Find(
                ActionContext.CurrentFacility,
                requestModel.Month,
                requestModel.Year,
                requestModel.PageNumber,
                requestModel.PageSize);

            // We are setting up the grid to provide data so we pass in the patient chart link formatter
            var gridModel = new PatientCensusGrid();
            ModelMapper.MapForReadOnly(PatientCensusQuery, gridModel);

            return gridModel.GetJsonResult();
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                var entity = PatientCensusRepository.Get(id.Value);

                if (entity != null)
                {
                    var formModel = ModelMapper.MapForUpdate<PatientCensusForm>(entity);

                    return View("Edit", formModel);
                }
            }

            return RedirectToAction("List");
        }

        [HttpPost, SupportsFormCancel]
        public ActionResult Edit(PatientCensusForm formModel, bool formCancelled)
        {

            try
            {
                if (formCancelled != true)
                {
                    var entity = PatientCensusRepository.Get(formModel.Id);

                    if (entity == null)
                    {
                        return RedirectToAction("List");
                    }

                    ModelMapper.MapForUpdate(formModel, entity);
                    Logger.Info("Updated census record for {2}  {1}-{0}", formModel.Month, formModel.Year, ActionContext.CurrentFacility.Name);
                }

                return RedirectToAction("List");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }


            return View("Edit", formModel);
        }

 


    }
}