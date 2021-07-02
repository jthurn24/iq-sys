using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Reporting.Repositories;
using System.IO;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.Web.Extensions
{
    public static class ControllerContextExtensions
    {
        public static void SetContentTitle(this ControllerContext context, string title)
        {
            context.Controller.ViewData["Layout_ContentTitle"] = title;
        }

        public static void SetContentTitle(this ControllerContext context, string title, params object[] args)
        {
            context.Controller.ViewData["Layout_ContentTitle"] = string.Format(title, args);
        }

        public static Guid GetDefaultQuarterId(this ControllerContext context, IDimensionRepository repository)
        {
            if (context.HttpContext.Session["ControllerContext.DefaultQuarterID"] == null)
            {
                return repository.GetNonFutureQuarters().FirstOrDefault().Id;
            }

            return (Guid)(context.HttpContext.Session["ControllerContext.DefaultQuarterID"]);
        }

        public static void SetDefaultQuarterId(this ControllerContext context, Guid id)
        {
            context.HttpContext.Session["ControllerContext.DefaultQuarterID"] = id;
        }

        public static Guid GetDefaultIncidentGroupId(this ControllerContext context, IDimensionRepository repository)
        {
            if (context.HttpContext.Session["ControllerContext.DefaultIncidentGroupID"] == null)
            {
                return repository.GetIncidentTypeGroups().First().Id;
            }

            return (Guid)(context.HttpContext.Session["ControllerContext.DefaultIncidentGroupID"]);
        }

        public static void SetDefaultIncidentGroupId(this ControllerContext context, Guid id)
        {
            context.HttpContext.Session["ControllerContext.DefaultIncidentGroupID"] = id;
        }

        public static byte[] GetFileData(this ControllerContext context, System.Web.HttpPostedFileBase file)
        {
            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }

            return data;
        }

        public static void SetUserMessage(this ControllerContext context, string message)
        {
            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            actionContext.SetUserMessage(message);
        }



    }
}