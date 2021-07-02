using System;
using System.Web.Mvc;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Mvc.ModelMapper;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Web.Models.Administration.WoundMap;
using System.Collections;
using IQI.Intuition.Web.Attributes;
using System.Collections.Generic;
using RedArrow.Framework.Mvc.Security;
using IQI.Intuition.Web.Extensions;

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess, RequiresSystemUserAttribute(Domain.Enumerations.SystemUserRole.Admin)]
    public class WoundMapController : Controller
    {
        public WoundMapController(
			IActionContext actionContext, 
			IModelMapper modelMapper, 
			IWoundRepository woundRepository,
            ISystemRepository systemRepository)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
			ModelMapper = modelMapper.ThrowIfNullArgument("modelMapper");
            WoundRepository = woundRepository.ThrowIfNullArgument("woundRepository");
            SystemRepository = systemRepository.ThrowIfNullArgument("systemRepository");
		}

		protected virtual IActionContext ActionContext { get; private set; }

		protected virtual IModelMapper ModelMapper { get; private set; }

        protected virtual IWoundRepository WoundRepository { get; private set; }

        protected virtual ISystemRepository SystemRepository { get; private set; }

        public ActionResult Index(int? siteId)
        {
            var sites = WoundRepository.AllSites;

            var model = new Edit();

            model.Graph = new Intuition.Reporting.Graphics.BodyGraph();


            var display = sites;

            if (siteId.HasValue)
            {
                var match = display.Where(X => X.Id == siteId.Value).First();
                display = display.Where(X => X.Name == match.Name);
            }

            foreach (var d in display)
            {
                model.Graph.Areas.Add(new Intuition.Reporting.Graphics.BodyGraph.Area()
                {
                    BottomRightX = d.BottomRightX.Value,
                    BottomRightY = d.BottomRightY.Value,
                    TopLeftX = d.TopLeftX.Value,
                    TopLeftY = d.TopLeftY.Value,
                    ShadingColor = System.Drawing.Color.Red,
                    ShadingOpacity = 100
                });
            }



            model.Sites = sites.Distinct(x => x.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
                .ToList();

            return View(model);
        }

    }
}
