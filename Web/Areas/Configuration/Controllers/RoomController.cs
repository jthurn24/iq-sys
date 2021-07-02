using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Configuration.Room;

namespace IQI.Intuition.Web.Areas.Configuration.Controllers
{
    public class RoomController : Controller
    {
        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual IFacilityRepository FacilityRepository { get; private set; }

        public RoomController(
            IActionContext actionContext,
            IFacilityRepository facilityRepository,
            IModelMapper modelMapper)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            FacilityRepository = facilityRepository.ThrowIfNullArgument("facilityRepository");
        }

        public ActionResult Index(int wingId)
        {
            var model = new RoomEntryList();
            model.Entries = new List<RoomEntry>();
            var wing = FacilityRepository.SearchWings(ActionContext.CurrentFacility.Id, wingId).FirstOrDefault();
            model.WingId = wingId;
            model.WingName = wing.Name;
            model.NewRoom = ModelMapper.MapForCreate<RoomEntry>();
            model.NewRoom.WingId = wingId;


            foreach (var m in wing.Rooms.OrderBy( x=> x.Name))
            {
                model.Entries.Add(ModelMapper.MapForUpdate<RoomEntry>(m));
            }

            return View(model);
        }

        public ActionResult Exit(int wingId)
        {
            var wing = FacilityRepository.SearchWings(ActionContext.CurrentFacility.Id, wingId).FirstOrDefault();
            return RedirectToAction("Index", "Wing", new { floorId = wing.Floor.Id });

        }


        [HttpPost]
        public ActionResult Add(RoomEntryList model, int wingId)
        {
            model.Entries = new List<RoomEntry>();
            var wing = FacilityRepository.SearchWings(ActionContext.CurrentFacility.Id, wingId).FirstOrDefault();
            model.WingId = wingId;
            model.WingName = wing.Name;

            try
            {
                var entity = new Room();
                ModelMapper.MapForCreate(model.NewRoom, entity);
                wing.AddRoom(entity);

                return RedirectToAction("Index", new { wingId = model.WingId });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }

            foreach (var m in wing.Rooms.OrderBy(x => x.Name))
            {
                model.Entries.Add(ModelMapper.MapForUpdate<RoomEntry>(m));
            }

            return View("Index", model);

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var entity = FacilityRepository.SearchRooms(ActionContext.CurrentFacility.Id, id).FirstOrDefault();
            var formModel =  this.ModelMapper.MapForUpdate<RoomEntry>(entity);
            return View("Edit", formModel);
        }

        [HttpPost]
        public ActionResult Edit(RoomEntry formModel, int id)
        {
            try
            {
                var entity = FacilityRepository.SearchRooms(ActionContext.CurrentFacility.Id, id).FirstOrDefault();
                this.ModelMapper.MapForUpdate(formModel,entity);

                return RedirectToAction("Index", new { wingId = formModel.WingId });
            }
            catch (ModelMappingException ex)
            {
                ModelState.AddModelMappingErrors(ex);
            }


            return View("Edit", formModel);

        }

        public ActionResult Inactivate(int id)
        {
            var entity = FacilityRepository.SearchRooms(ActionContext.CurrentFacility.Id, id).FirstOrDefault();
            entity.IsInactive = true;
            return RedirectToAction("Index", new { wingId = entity.Wing.Id });
        }

        public ActionResult Activate(int id)
        {
            var entity = FacilityRepository.SearchRooms(ActionContext.CurrentFacility.Id, id).FirstOrDefault();
            entity.IsInactive = false;
            return RedirectToAction("Index", new { wingId = entity.Wing.Id });
        }

    }
}
