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
using IQI.Intuition.Infrastructure.Services.QiCast;
using IQI.Intuition.Infrastructure.Services.QiCast.Commands;

namespace IQI.Intuition.Web.Areas.Configuration.Controllers
{
    public class QICastController : Controller
    {
        private Facility _Facility;

        public QICastController(
            IActionContext actionContext, 
            IModelMapper modelMapper,
            ICubeRepository cubeRepository,
            IDimensionRepository dimensionRepository,
            IUserRepository userRepository)
        {
            ActionContext = actionContext.ThrowIfNullArgument("actionContext");
            ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            CubeRepository = cubeRepository;
            DimensionRepository = dimensionRepository;

            _Facility = DimensionRepository.GetFacility(ActionContext.CurrentFacility.Guid);
            UserRepository = userRepository;

            CommandQueService = new CommandQueService(UserRepository);
        }

        protected virtual IActionContext ActionContext { get; private set; }
        protected virtual IModelMapper ModelMapper { get; private set; }
        protected virtual ICubeRepository CubeRepository { get; private set; }
        protected virtual IDimensionRepository DimensionRepository { get; private set; }
        protected virtual IUserRepository UserRepository { get; private set; }
        protected virtual CommandQueService CommandQueService { get; private set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var data = UserRepository.GetQICasts(ActionContext.CurrentFacility.Guid);
            return PartialView(data);
        }

        public ActionResult Add(string name)
        {
            var model = new IQI.Intuition.Reporting.Models.User.QICast();
            model.Id = RedArrow.Framework.Utilities.GuidHelper.NewGuid();
            model.FacilityId = ActionContext.CurrentFacility.Guid;
            model.Name = name;

            UserRepository.Update(model);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SetName(Guid id, string name)
        {
            var data = UserRepository
                .GetQICasts(ActionContext.CurrentFacility.Guid)
                .Where(x => x.Id == id)
                .First();

            data.Name = name;

            UserRepository.Update(data);

            return Json(new { success=true }, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult SetMessage(Guid id, string message)
        {
            var data = UserRepository
                .GetQICasts(ActionContext.CurrentFacility.Guid)
                .Where(x => x.Id == id)
                .First();

            data.Message = message;

            UserRepository.Update(data);

            if (data.Inactive == false)
            {
                CommandQueService.SendCommand(new MessageChange(message), data.Id);
            }


            return Json(new { success = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SetInactive(Guid id, bool val)
        {
            var data = UserRepository
                .GetQICasts(ActionContext.CurrentFacility.Guid)
                .Where(x => x.Id == id)
                .First();

            data.Inactive = val;

            UserRepository.Update(data);

            if (val)
            {
                CommandQueService.SendCommand(new MessageChange(string.Empty), data.Id);
                CommandQueService.SendCommand(new HideActiveSection(), data.Id);
            }
            else
            {
                CommandQueService.SendCommand(new MessageChange(data.Message), data.Id);
                CommandQueService.SendCommand(new ShowActiveSection(), data.Id);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
