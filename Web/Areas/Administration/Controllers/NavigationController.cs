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

namespace IQI.Intuition.Web.Areas.Administration.Controllers
{
    [AnonymousAccess]
    public class NavigationController : Controller
    {
        public NavigationController(
			IActionContext actionContext)
		{
			ActionContext = actionContext.ThrowIfNullArgument("actionContext");
		}

		protected virtual IActionContext ActionContext { get; private set; }

        public ActionResult AdministrationPanel()
        {
            if (ActionContext.CurrentSystemUser != null)
            {
                return PartialView(true);
            }

            return PartialView(false);
        }

    }
}
