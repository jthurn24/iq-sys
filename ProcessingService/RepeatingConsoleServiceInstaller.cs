using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using IQI.Intuition.Infrastructure.Services;

namespace IQI.Intuition.ProcessingService
{
    [RunInstaller(true)]
    public class RepeatingConsoleServiceInstaller : Installer
    {

            public RepeatingConsoleServiceInstaller()
            {
                AddService<IQI.Intuition.Infrastructure.Services.Exporting.ExportDirector>();
                AddService<IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.IncrementalService>();
                AddService<IQI.Intuition.Infrastructure.Services.Warnings.CoreWarningDirector>();
                AddService<IQI.Intuition.Infrastructure.Services.Notification.EmailNotificationDirector>();
                AddService<IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.CubeJobService>();
                AddService<IQI.Intuition.Infrastructure.Services.Notification.SMSNotificationDirector>();

                Installers.Add(new ServiceProcessInstaller() { Account = ServiceAccount.LocalSystem });
            }

            private void AddService<T>() where T : IConsoleService
            {
                var environment = System.Configuration.ConfigurationSettings.AppSettings["ServiceSuffix"];
                var i = new System.ServiceProcess.ServiceInstaller();
                i.StartType = ServiceStartMode.Automatic;
                i.ServiceName = string.Concat("IQI - ", typeof(T).Name, " ", environment);
                this.Installers.Add(i);

            }
    }
}
