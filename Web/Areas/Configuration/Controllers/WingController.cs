using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Configuration.Wing;

namespace IQI.Intuition.Web.Areas.Configuration.Controllers
{
    public class WingController : Controller
    {
      protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IFacilityRepository FacilityRepository { get; private set; }

        public WingController(
            IActionContext actionContext,
            IFacilityRepository facilityRepository,
            IModelMapper modelMapper)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            FacilityRepository = facilityRepository.ThrowIfNullArgument("facilityRepository");
        }

        public ActionResult Index(int floorId)
        {
            var model = new WingEntryList();
            model.Entries = new List<WingEntry>();
            var floor = ActionContext.CurrentFacility.Floors.Where(x => x.Id == floorId).FirstOrDefault();
            model.FloorId = floorId;
            model.FloorName = floor.Name;

            foreach (var m in floor.Wings.OrderBy(x => x.Name))
            {
                model.Entries.Add(ModelMapper.MapForUpdate<WingEntry>(m));
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Add(int floorId)
        {
            var formModel = ModelMapper.MapForCreate<WingEntry>();
            formModel.FloorId = floorId;
            return View("Edit", formModel);
        }

        [HttpPost]
        public ActionResult Add(WingEntry formModel, int floorId)
        {
            try
            {
                var entity = new Wing();
                ModelMapper.MapForCreate(formModel, entity);

                var floor = ActionContext.CurrentFacility.Floors.Where(x => x.Id == floorId).FirstOrDefault();
                floor.AddWing(entity);

                return RedirectToAction("Index", new { floorId = formModel.FloorId });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }


            return View("Edit", formModel);

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var entity = FacilityRepository.SearchWings(ActionContext.CurrentFacility.Id, id).FirstOrDefault();
            var formModel =  this.ModelMapper.MapForUpdate<WingEntry>(entity);
            return View("Edit", formModel);
        }

        [HttpPost]
        public ActionResult Edit(WingEntry formModel, int id)
        {
            try
            {
                var entity = FacilityRepository.SearchWings(ActionContext.CurrentFacility.Id, id).FirstOrDefault();
                this.ModelMapper.MapForUpdate(formModel,entity);

                return RedirectToAction("Index", new { floorId = formModel.FloorId });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }


            return View("Edit", formModel);

        }


    }
}
