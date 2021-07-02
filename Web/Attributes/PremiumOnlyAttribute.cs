using IQI.Intuition.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace IQI.Intuition.Web.Attributes
{
    public class PremiumOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();

            if(actionContext.CurrentFacility.FreeMode())
            {
                filterContext.RequestContext.HttpContext.Items["PermiumOnly"] = "1";
            }
        }
    }
}