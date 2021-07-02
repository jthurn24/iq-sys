using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedArrow.Framework.Ioc.StructureMap;
using IQI.Intuition.Infrastructure.Ioc;
using IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService;
using IQI.Intuition.Infrastructure.Services.Development;
using IQI.Intuition.Infrastructure.Services.Exporting;
using IQI.Intuition.Infrastructure.Services.Utilities;
using IQI.Intuition.Infrastructure.Services.Warnings;
using IQI.Intuition.Infrastructure.Services.Notification;

namespace IQI.Intuition.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            using (var container = StructureMapConfig.Configure(StructureMapConfig.DataContextMode.Stateless))
            {
                if (args[0] == "IncrementalSynchronization")
                {
                        var syncService = container.GetInstance<IncrementalService>();
                        syncService.Run(args);
                }
                if (args[0] == "CubeJobService")
                {
                    var syncService = container.GetInstance<CubeJobService>();
                    syncService.Run(args);
                }
                else if (args[0] == "DailySynchronization")
                {
                    var syncService = container.GetInstance<DailyService>();
                    syncService.Run(args);
                }
                else if (args[0] == "ExportDirector")
                {
                    var exportService = container.GetInstance<ExportDirector>();
                    exportService.Run(args);
                }
                else if (args[0] == "EmailNotificationDirector")
                {
                    var exportService = container.GetInstance<EmailNotificationDirector>();
                    exportService.Run(args);
                }
                else if (args[0] == "SMSNotificationDirector")
                {
                    var exportService = container.GetInstance<SMSNotificationDirector>();
                    exportService.Run(args);
                }
                else if (args[0] == "FacilityManagement")
                {
                    var exportService = container.GetInstance<FacilityManagement>();
                    exportService.Run(args);
                }
                else if (args[0] == "CoreWarningDirector")
                {
                    var exportService = container.GetInstance<CoreWarningDirector>();
                    exportService.Run(args);
                }
                else if (args[0] == "WIILI")
                {
                    var updateService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Migration.InfectionCriteria.V3WI>();
                    updateService.Run(args);
                }
                else if (args[0] == "FixUTI")
                {
                    var fi = container.GetInstance<IQI.Intuition.Infrastructure.Services.Migration.InfectionCriteria.FixUti>();
                    fi.Run(args);
                }
                else if (args[0] == "SystemUser")
                {
                    var updateService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.AddSystemUser>();
                    updateService.Run(args);
                }
                else if (args[0] == "ImportDMG")
                {
                    var importService = container.GetInstance<IQI.Intuition.Infrastructure.Services.LeadGeneration.ImportDMG>();
                    importService.Run(args);
                }
                else if (args[0] == "IntegrityCheck")
                {
                    var integrityService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Reporting.IntegrityService.ScanService>();
                    integrityService.Run(args);
                }
                else if (args[0] == "CalculateAverages")
                {
                    var integrityService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Reporting.AveragesService.InfectionAverageService>();
                    integrityService.Run(args);
                }
                else if (args[0] == "InstallScripts")
                {
                    string constring = System.Configuration.ConfigurationManager.ConnectionStrings["IQI.Intuition.Domain.Models"].ConnectionString;
                    var connection = new System.Data.SqlClient.SqlConnection(constring);
                    var service = new SnyderIS.sCore.Migration.ScriptInstaller(connection, args[1]);
                }
                else if (args[0] == "SyncFreshBooks")
                {
                    var fbSyncService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.SyncFreshBooks>();
                    fbSyncService.Run(args);
                }
                else if (args[0] == "SyncFreshBooksInvoices")
                {
                    var fbiSyncService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.SyncFreshBooksInvoices>();
                    fbiSyncService.Run(args);
                }
                else if (args[0] == "SyncFreshBooksInvoices2")
                {
                    var fbi2SyncService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.SyncFreshBooksInvoices2>();
                    fbi2SyncService.Run(args);
                }
                else if (args[0] == "ImportDailyMed")
                {
                    var dmService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Importing.Drugs.ImportDailyMed>();
                    dmService.Run(args);
                }
                else if (args[0] == "SecureData2")
                {
                    var dmService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.SecureData2>();
                    dmService.Run(args);
                }
                else if (args[0] == "MonthlyAuditReport")
                {
                    var dmService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.AuditEmails>();
                    dmService.Run(args);
                }
                else if (args[0] == "SyncWoundSites")
                {
                    var swService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.SyncWoundSites>();
                    swService.Run(args);
                }
                else if (args[0] == "EvalWoundSites")
                {
                    var ewService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.EvalWoundSites>();
                    ewService.Run(args);
                }
                else if (args[0] == "pud")
                {
                    var tfService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.ProductUtilizationDump>();
                    tfService.Run(args);
                }
                else if (args[0] == "aconvert")
                {
                    var acService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Migration.ConvertAverages>();
                    acService.Run(args);
                }
                else if (args[0] == "cleanup")
                {
                    var clService = container.GetInstance<IQI.Intuition.Infrastructure.Services.Utilities.Cleanup>();
                    clService.Run(args);
                }
                else
                {
                    System.Console.WriteLine("Invalid Command");
                }
            }

        }
    }
}
