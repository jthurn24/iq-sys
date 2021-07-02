using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Infrastructure.Services.Implementation
{
    public class WebViewEngine : System.Web.Mvc.RazorViewEngine
    {
        public override System.Web.Mvc.ViewEngineResult FindView(System.Web.Mvc.ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext.HttpContext.Request["export"] != null && controllerContext.HttpContext.Request["export"].ToLower() == "true")
            {
                viewName = string.Concat(viewName, ".export");
                masterName = "~/Views/Shared/_ExportLayout.cshtml";
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }
    }
}
