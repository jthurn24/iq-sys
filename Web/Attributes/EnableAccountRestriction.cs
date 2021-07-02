using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IQI.Intuition.Domain.Utilities;
using IQI.Intuition.Infrastructure.Services;
namespace IQI.Intuition.Web.Attributes
{
    public class EnableAccountRestriction : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            IQI.Intuition.Infrastructure.Services.Protection.AccountWatchDogService.EnableProtection();
            base.OnActionExecuting(filterContext);
        }
    }
}