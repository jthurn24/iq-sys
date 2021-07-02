using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IQI.Intuition.Domain.Utilities;
using IQI.Intuition.Infrastructure.Services;
using RedArrow.Framework.Mvc.Security;

namespace IQI.Intuition.Web.Attributes
{
    public class RequiresSystemUserAttribute : ActionFilterAttribute
    {
        private Domain.Enumerations.SystemUserRole? _RequiredRole;

        public RequiresSystemUserAttribute()
        {
        }

        public RequiresSystemUserAttribute(Domain.Enumerations.SystemUserRole role)
        {
            _RequiredRole = role;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();

            if (actionContext.CurrentSystemUser == null)
            {
                filterContext.HttpContext.Response.RedirectToRoute(new { controller="Authentication", action="SignIn", area="Administration" });
            }

            if (_RequiredRole.HasValue)
            {
                if (actionContext.CurrentSystemUser.Role != _RequiredRole)
                {
                    throw new Exception("Access denied");
                }
            }
        }
    }
}