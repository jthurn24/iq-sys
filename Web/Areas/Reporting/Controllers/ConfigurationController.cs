using System;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Collections.Generic;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Web.Attributes;
using IQI.Intuition.Web.Models.Reporting.Configuration;
using IQI.Intuition.Reporting.Graphics;
using System.Drawing;
using SnyderIS.sCore.Persistence;
using System.IO;

namespace IQI.Intuition.Web.Areas.Reporting.Controllers
{
    public class ConfigurationController : Controller
    {
        
        private Facility _Facility;
        private IDocumentStore _Store;

        public ConfigurationController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            ICubeRepository cubeRepository,
            IDimensionRepository dimensionRepository,
            IDocumentStore store)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            CubeRepository = cubeRepository;
            DimensionRepository = dimensionRepository;

            _Facility = DimensionRepository.GetFacility(ActionContext.CurrentFacility.Guid);
            _Store = store;
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual ICubeRepository CubeRepository { get; private set; }
        protected virtual IDimensionRepository DimensionRepository { get; private set; }

        public ActionResult FloorMapConfiguration(FloorMapConfiguration model)
        {
            model.FloorMapOptions = DimensionRepository
                .GetFloorMapsForFacility(_Facility.Id)
                .Where(x => x.Active == true)
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });

            if (model.SelectedFloorMap.HasValue == false)
            {
                model.SelectedFloorMap = DimensionRepository.GetFloorMapsForFacility(_Facility.Id).First().Id;
            }

            model.Points = new List<FloorMapConfigurationPoint>();

            var rooms = DimensionRepository.GetFloorMapRoomsForFloorMap(model.SelectedFloorMap.Value);

            foreach (var room in rooms)
            {
                model.Points.Add(new FloorMapConfigurationPoint() { Coordinates = room.Coordinates, ID = room.Id, RoomName = room.Room.Name });
            }

            return View(model);
        }

        public ActionResult UploadFloorMap(Guid id)
        {
            HttpPostedFileBase file = Request.Files["FileUpload"];
            if (file.ContentLength > 0)
            {
                var map = DimensionRepository.GetFloorMap(id);
                var i = System.Drawing.Image.FromStream(file.InputStream);

                var mapImage = DimensionRepository.GetFloorMapImage(id);

                if (mapImage == null)
                {
                    mapImage = new FloorMapImage();
                    mapImage.FloorMap = map;
                    mapImage.Id = RedArrow.Framework.Utilities.GuidHelper.NewGuid();
                }

                MemoryStream ms = new MemoryStream();
                i.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                mapImage.Image = ms.ToArray();
                _Store.Save(mapImage);


            }

            return RedirectToAction("FloorMapConfiguration");
        }

        public ActionResult FloorMapConfigurationUpdate(Guid id, string coordinates)
        {
            var room = DimensionRepository.GetFloorMapRoom(id);
            room.Coordinates = coordinates;
            _Store.Save(room);

            return Json(new { Result = true},JsonRequestBehavior.AllowGet);
        }

        public ActionResult FloorMapImage(Guid id)
        {
            var chart = new FloorMapChart(id);

            var rooms = DimensionRepository.GetFloorMapRoomsForFloorMap(id);

            foreach (var room in rooms)
            {
                chart.Circles.Add(new FloorMapChart.Circle() { Coordinates = room.Coordinates, ShadingOpacity = 255, ShadingColor = Color.Blue, Width = 20 });
            }

            var stream = FloorMapChart.GenerateImage(chart.SerializeForRender(), _Store);
            return File(stream, "image/jpeg");
         
        }

    }
}
