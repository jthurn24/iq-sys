using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using IQI.Intuition.Infrastructure.Services;
using IQI.Intuition.Infrastructure.Ioc;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.ObjectModel.AuditTracking;
using RedArrow.Framework.Logging;
using System.Reflection;

namespace IQI.Intuition.ProcessingService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            //HibernatingRhinos.Profiler.Appender.ProfilerIntegration.IgnoreAll();

            if (args.Count() < 1)
            {

                var container = StructureMapConfig.Configure(StructureMapConfig.DataContextMode.Stateless);
                container.Inject<IUserNameProvider>(new UserNameProvider());

                var log = container.GetInstance<ILog>();

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
                    new RepeatingConsoleService<IQI.Intuition.Infrastructure.Services.Exporting.ExportDirector>(container,log),
                    new RepeatingConsoleService<IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.IncrementalService>(container,log),
                    new RepeatingConsoleService<IQI.Intuition.Infrastructure.Services.Warnings.CoreWarningDirector>(container,log),
                    new RepeatingConsoleService<IQI.Intuition.Infrastructure.Services.Notification.EmailNotificationDirector>(container,log),
                    new RepeatingConsoleService<IQI.Intuition.Infrastructure.Services.Notification.SMSNotificationDirector>(container,log),
                    new RepeatingConsoleService<IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.CubeJobService>(container,log)
			    };
                ServiceBase.Run(ServicesToRun);
            }
            else if (args[0] == "/i")
            {
                System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
            }
            else if (args[0] == "/u")
            {
                System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
            }

        }
    }
}
