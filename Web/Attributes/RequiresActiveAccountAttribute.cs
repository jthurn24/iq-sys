using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IQI.Intuition.Domain.Utilities;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Web.Attributes
{
    public class RequiresActiveAccountAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();

            if (actionContext.CurrentFacility == null)
            {
                filterContext.HttpContext.Response.Redirect("http://www.iqisystems.com");
            }
        }
    }
}