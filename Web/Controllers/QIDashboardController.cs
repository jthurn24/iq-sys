using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Exi.Mvc.Controllers;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Web.Controllers
{
    public class QIDashboardController : Canvas
    {

        private IUserRepository _UserRepository { get; set; }
        private IActionContext _ActionContext { get; set; }
        private IDimensionRepository _DimensionRepository { get; set; }

        public QIDashboardController(IUserRepository userRepository,
            IActionContext actionContext,
            IDimensionRepository dimensionRepository)
        {
            _UserRepository = userRepository;
            _ActionContext = actionContext;
            _DimensionRepository = dimensionRepository;

        }

        public ActionResult Index()
        {
            var name = string.Concat(_ActionContext.CurrentUser.FirstName,
                " ",
                _ActionContext.CurrentUser.LastName);

            return View((object)name);
        }


        public override SnyderIS.sCore.Exi.Interfaces.Canvas.ICanvas GetCanvas()
        {
            return new IQI.Intuition.Exi.Canvases.QIDashboard.Canvas(
                _ActionContext.CurrentUser.Guid,
                _UserRepository);
        }

        public override SnyderIS.sCore.Exi.Interfaces.Widget.IWizardBuilder GetWizardBuilder()
        {
            return new IQI.Intuition.Exi.Configuration.QIDashboard.WizardBuilder(
            _ActionContext, _DimensionRepository);
        }
    }
}
