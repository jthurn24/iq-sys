using System;
using System.Web.Mvc;
using System.Web.Routing;
using RedArrow.Framework.Mvc;
using RedArrow.Framework.Persistence.Mvc;
using IQI.Intuition.Infrastructure;


namespace IQI.Intuition.Web
{
    public class MvcApplication : UnitOfWorkPerRequestHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            FilterProviders.Providers.Add(new GlobalAttributeFilterProvider<AuthorizeAttribute>());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.RouteExistingFiles = true;

            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.([iI][cC][oO]|[gG][iI][fF])(/.*)?" });
            routes.IgnoreRoute("content/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
            routes.IgnoreRoute("elmah.axd");

            routes.MapRoute(
                "SignIn", // Route name
                "login", // URL with parameters
                new { controller = "Authentication", action = "SignIn" } ,
                new[] { "IQI.Intuition.Web.Controllers" }
            );

            routes.MapRoute(
                "AccessDenied", // Route name
                "noaccess", // URL with parameters
                new { controller = "Authentication", action = "AccessDenied" },
                new[] { "IQI.Intuition.Web.Controllers" }
            );

            routes.MapRoute(
                "Error", // Route name
                "error", // URL with parameters
                new { controller = "Home", action = "Error" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "IQI.Intuition.Web.Controllers" }
            );
        }



        protected void Application_Start()
        {
            string setting = System.Configuration.ConfigurationSettings.AppSettings["RequireSSL"];
            bool requiresSSL = false;
            Boolean.TryParse(setting, out requiresSSL);

            if (requiresSSL)
            {
                GlobalFilters.Filters.Add(new RequireHttpsAttribute());
            }

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            /* Use our custom razor based ViewEngine */
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new IQI.Intuition.Infrastructure.Services.Implementation.WebViewEngine());

            /* Init IOC */
            BootStrapper.Initialize(Infrastructure.Ioc.StructureMapConfig.DataContextMode.UnitOfWork);

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

           
        }
    }
}