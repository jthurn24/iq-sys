using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Web.Extensions
{
    public static class ViewContextExtensions
    {
        private static string _EnvironmentDetails = string.Empty;

        public static bool HasErrors(this ViewContext context)
        {
            if (context.ViewData.ModelState.Values.Where(v => v.Errors.Count != 0).Count() > 0)
            {
                return true;
            }

            return false;
        }

        public static bool HasProduct(this ViewContext context, Domain.Enumerations.KnownProductType productType)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            return actionContext.CurrentFacility.HasProduct(productType);
        }

        public static string EnvironmentDetails(this ViewContext context)
        {
            if (_EnvironmentDetails == string.Empty)
            {
                lock (_EnvironmentDetails)
                {
                    var actionContext = DependencyResolver.Current.GetService<IActionContext>();
                    _EnvironmentDetails = string.Concat("Build: ",actionContext.BuildVersion, " Env: ", actionContext.EnvironmentName, " Srv: ", actionContext.ServerName);
                }
            }

            return _EnvironmentDetails;

        }


        public static bool UserHasPermission(this ViewContext context, Domain.Enumerations.KnownPermision permission)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            var user = actionContext.CurrentUser;
            return user.HasPermission(permission);
        }

        public static bool SystemUserInRole(this ViewContext context, Domain.Enumerations.SystemUserRole role)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            var user = actionContext.CurrentSystemUser;

            if (user != null)
            {
                return user.Role == role;
            }

            return false;
        }

        public static string GetUserName(this ViewContext context)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            var user = actionContext.CurrentUser;
            
           
            if(user != null)
            {
                return string.Concat(user.FirstName, " ", user.LastName);
            }

            return string.Empty;
        }


        public static string GetFacilityName(this ViewContext context)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();

            return actionContext.CurrentFacility.Name;


        }

        public static string LastSynchronized(this ViewContext context)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();

            if(actionContext.CurrentFacility.LastSynchronizedAt.HasValue == false)
            {
                return "N/A";
            }

            return string.Concat(
                actionContext.CurrentFacility.LastSynchronizedAt.Value.ToShortTimeString()
                , " "
                , actionContext.CurrentFacility.LastSynchronizedAt.Value.ToShortDateString()
                , " CST ");
        }

        public static bool HasPendingUserMessage(this ViewContext context)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            return actionContext.GetUserMessage().IsNotNullOrEmpty();
        }



        public static string GetPendingUserMessage(this ViewContext context)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            var message = actionContext.GetUserMessage();
            actionContext.ClearUserMessage();
            return message;
        }

        public static string GetTrackerUrl(this ViewContext context)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["PageTrackerUrl"] != null)
            {
                return System.Configuration.ConfigurationManager.AppSettings["PageTrackerUrl"];
            }

            var helper = new UrlHelper(context.RequestContext);

            return helper.Action("TPV", "Home", new { area = "" });

        }
    }
}