using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Configuration.Floor;

namespace IQI.Intuition.Web.Areas.Configuration.Controllers
{
    public class FloorController : Controller
    {
        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }

        public FloorController(
            IActionContext actionContext,
            IModelMapper modelMapper)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
        }

        public ActionResult Index()
        {
            var model = new FloorEntryList();
            model.Entries = new List<FloorEntry>();

            foreach (var m in this.ActionContext.CurrentFacility.Floors.OrderBy(x => x.Name))
            {
                model.Entries.Add(ModelMapper.MapForUpdate<FloorEntry>(m));
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            var formModel = ModelMapper.MapForCreate<FloorEntry>();
            return View("Edit", formModel);
        }

        [HttpPost]
        public ActionResult Add(FloorEntry formModel)
        {
            try
            {
                var entity = new Floor();
                entity.Facility = ActionContext.CurrentFacility;
                ModelMapper.MapForCreate(formModel, entity);

                ActionContext.CurrentFacility.AddFloor(entity);

                return RedirectToAction("Index");
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
            var entity = this.ActionContext.CurrentFacility.Floors.Where(x => x.Id == id).First();
            var formModel =  this.ModelMapper.MapForUpdate<FloorEntry>(entity);
            return View("Edit", formModel);
        }

        [HttpPost]
        public ActionResult Edit(FloorEntry formModel, int id)
        {
            try
            {
                var entity = this.ActionContext.CurrentFacility.Floors.Where(x => x.Id == id).First();
                this.ModelMapper.MapForUpdate(formModel,entity);

                return RedirectToAction("Index");
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }


            return View("Edit", formModel);

        }


    }
}
