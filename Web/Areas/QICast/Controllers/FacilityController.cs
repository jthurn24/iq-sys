using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.Account;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;
using IQI.Intuition.Web.Models.QICast;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Infrastructure.Services.QiCast;
using IQI.Intuition.Infrastructure.Services.QiCast.Commands;

namespace IQI.Intuition.Web.Areas.QICast.Controllers
{
    [AnonymousAccess]
    public class FacilityController : SnyderIS.sCore.Exi.Mvc.Controllers.Canvas
    {
        private IUserRepository _UserRepository;
        private IActionContext _Context;
        private IDimensionRepository _DimensionRepository;
        private CommandQueService _CommandQueService;

        public FacilityController(IUserRepository userRepository,
            IActionContext context,
            IDimensionRepository dimensionRepository)
        {
            _UserRepository = userRepository;
            _Context = context;
            _DimensionRepository = dimensionRepository;
            _CommandQueService = new CommandQueService(_UserRepository);
        }

        public ActionResult Index(Guid? id, bool? designMode)
        {
            Response.Cookies.Add(
                new System.Web.HttpCookie("ScreenGuid", id.ToString()) 
                { Expires = DateTime.Today.AddDays(10000) });

            var model = new FacilityView();
            model.AccountName = _Context.CurrentAccount.Name;
            model.FacilityName = _Context.CurrentFacility.Name;
            model.DesignMode = designMode.HasValue ? designMode.Value : false;

            return View(model);
        }

        public ActionResult GetCommands(bool initial)
        {
            return Json(new { 
                commands = _CommandQueService.GetCommands(initial,GetScreenGuid()) 
            }, JsonRequestBehavior.AllowGet);
        }



        private Guid GetScreenGuid()
        {
            return new Guid(Request.Cookies["ScreenGuid"].Value);
        }

        public override SnyderIS.sCore.Exi.Interfaces.Canvas.ICanvas GetCanvas()
        {
            var canvas = new IQI.Intuition.Exi.Canvases.QICast.Canvas(GetScreenGuid(), _UserRepository);
            return canvas;
        }

        public override SnyderIS.sCore.Exi.Interfaces.Widget.IWizardBuilder GetWizardBuilder()
        {
            return new IQI.Intuition.Exi.Configuration.QICast.WizardBuilder(
                _Context, _DimensionRepository);
        }

        protected override void OnStateChange()
        {
            _CommandQueService.SendCommand(
                new RefreshComponents(), GetScreenGuid());

        }
    }
}
