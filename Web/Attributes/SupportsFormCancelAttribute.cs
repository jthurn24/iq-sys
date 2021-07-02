using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace IQI.Intuition.Web.Attributes
{
    public class SupportsFormCancelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool formCancelled = false;
            if (filterContext.HttpContext.Request["cancel"] != null)
            {
                formCancelled = true;
            }
            filterContext.ActionParameters["formCancelled"] = formCancelled;
            base.OnActionExecuting(filterContext);
        }
    }
}