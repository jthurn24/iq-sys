using System;
using System.Web.Mvc;
using RedArrow.Framework.Ioc.StructureMap;

namespace IQI.Intuition.Infrastructure
{
    using IQI.Intuition.Infrastructure.Ioc;

    public static class BootStrapper
    {
        private static bool _IsIntialized = false;
        private static readonly object _Lock = new object();

        public static void Initialize(StructureMapConfig.DataContextMode dataContextMode)
        {
            if (_IsIntialized)
            {
                return;
            }

            lock (_Lock)
            {
                if (_IsIntialized)
                {
                    return;
                }

                // Configure the global IoC container
                var container = StructureMapConfig.Configure(dataContextMode);

                // Configure the ASP.NET MVC dependency resolver to use the global IoC container
                DependencyResolver.SetResolver(new MvcDependencyResolver(container));

                _IsIntialized = true;
            }
        }
    }
}