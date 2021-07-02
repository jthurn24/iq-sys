using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Extensions.Formatting;
using SnyderIS.sCore.Persistence;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using RedArrow.Framework.ObjectModel.AuditTracking;

namespace IQI.Intuition.Infrastructure.Services.Reporting.IntegrityService
{
    public class ScanService : IConsoleService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private IDimensionRepository _DimensionRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IStatelessDataContext _DataContext;
        private IFactBuilderRepository _FactBuilderRespository;
        private ConsoleLogWrapper _Log;
        private ISessionActivator _SessionActivator;
        private const int MAX_VARIANCES = 20;
        private AuditTrackingWorker _AuditWorker;
        private IDocumentStore _Store;

        public ScanService(
                IDimensionBuilderRepository dimensionBuilderRepository,
                IDimensionRepository dimensionRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IStatelessDataContext statelessDataContext,
                IStatelessDataContext dataContext,
                IUnitOfWork unitOfWork,
                IFactBuilderRepository factBuilderRespository,
                ILog log,
                ISessionActivator sessionActivator,
                AuditTrackingWorker auditWorker,
                IDocumentStore store
            )
        {
            _DimensionBuilderRepository = dimensionBuilderRepository;
            _CubeBuilderRepository = cubeBuilderRepository;
            _FactBuilderRespository = factBuilderRespository;
            _DimensionRepository = dimensionRepository;
            _DataContext = dataContext;
            _Log = new ConsoleLogWrapper(log);
            _SessionActivator = sessionActivator;
            _AuditWorker = auditWorker;
            _Store = store;
        }


        public void Run(string[] args)
        {
            _Log.Info("Database integrity check started");

            var emailMessageBuilder = new System.Text.StringBuilder();
            int varianceCounter = 0;

            var facilities = _DataContext.CreateQuery<Domain.Models.Facility>().FetchAll();
            int counter = 0;

            int days = 30;
            bool includeRoomDays = true;

            if (args.Length > 2)
            {
                days = Convert.ToInt32(args[1]);
                includeRoomDays = Convert.ToBoolean(args[2]);
            }

            foreach (var facility in facilities)
            {
                counter ++;

                System.Console.WriteLine("Variances detected so far: {0}", varianceCounter);
                System.Console.WriteLine("Scanning facility {0}  ({1} of {2}",facility.Name,counter,facilities.Count());

                try
                {
                    _Log.Info("Validating facility", facility.Id);

                    var serviceList = new List<IScanService>();

                    //serviceList.Add(new Incident.CubeServices.FacilityMonthIncidentDayOfWeek(
                    //    _DimensionBuilderRepository,
                    //    _CubeBuilderRepository,
                    //    _DataContext,
                    //    _FactBuilderRespository,
                    //    _Log,
                    //    _Store));


                    //serviceList.Add(new Incident.CubeServices.FacilityMonthIncidentHourOfDay(
                    //    _DimensionBuilderRepository,
                    //    _CubeBuilderRepository,
                    //    _DataContext,
                    //    _FactBuilderRespository,
                    //    _Log,
                    //    _Store));


                    //serviceList.Add(new Incident.CubeServices.FacilityMonthIncidentInjuryLevel(
                    //    _DimensionBuilderRepository,
                    //    _CubeBuilderRepository,
                    //    _DataContext,
                    //    _FactBuilderRespository,
                    //    _Log,
                    //    _Store));



                    if (includeRoomDays)
                    {
                        serviceList.Add(new Infection.CubeServices.FloorMapRoomDayInfectionType(
                            _DimensionBuilderRepository,
                            _CubeBuilderRepository,
                            _DataContext,
                            _FactBuilderRespository,
                            _Log,
                            _Store));
                    }

                    serviceList.Add(new Infection.CubeServices.FacilityMonthInfectionSite(
                        _DimensionBuilderRepository,
                        _CubeBuilderRepository,
                        _DataContext,
                        _FactBuilderRespository,
                        _Log,
                        _Store));

                    var rFacility = _DimensionBuilderRepository.GetOrCreateFacility(facility.Guid);

                    var variances = RunBatch(serviceList, facility, rFacility, days);

                    foreach (var variance in variances)
                    {
                        varianceCounter++;

                        if (varianceCounter < MAX_VARIANCES)
                        {
                            var template = System.Text.Encoding.UTF8.GetString(Properties.Resources.EmailTemplate_VarianceDetails);
                            emailMessageBuilder.Append(RazorEngine.Razor.Parse(template, variance));
                        }
                    }

                    if (emailMessageBuilder.ToString() != string.Empty)
                    {
                        var email = new Domain.Models.SystemEmailNotification();
                        email.SendTo = Domain.Constants.SYSTEM_DIAGNOSTIC_EMAIL;
                        email.Subject = "Reporting Integrity Variance Discovery ";
                        email.MessageText = emailMessageBuilder.ToString();
                        email.IsHtml = true;
                        _DataContext.Insert(email);
                    }


                    emailMessageBuilder.Clear();

                }
                catch (Exception ex)
                {
                    _Log.Error(ex);
                    System.Console.WriteLine(ex.Message);
                }

            }



            _Log.Info("Database integrity check finished");
        }


        private IEnumerable<VarianceDetails> RunBatch(
            IEnumerable<IScanService> services,
            Domain.Models.Facility dfacility,
            IQI.Intuition.Reporting.Models.Dimensions.Facility rFacility,
            int scanDays)
        {
            var variances = new List<VarianceDetails>();

            foreach (var service in services)
            {
                service.Run(dfacility, rFacility, variances, scanDays);
            }

            return variances;
        }

    }


    public class ConsoleLogWrapper : ILog
    {
        private ILog _Logger;
        private bool _StatusMode = false;

        public ConsoleLogWrapper(ILog logger)
        {
            _Logger = logger;
        }

        public void SetStatus(string message)
        {
            if (!_StatusMode)
            {
                _StatusMode = true;
                System.Console.Write(Environment.NewLine);
            }

            Console.Write(string.Concat("\r",message.PadRight(75,' ')));
        }

        public void ClearStatus()
        {
            _StatusMode = false;
            Console.Write(string.Concat("\r", string.Empty.PadRight(75, ' ')));
            Console.Write("\r");
        }

        public void Error(string messageFormat, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(messageFormat, args);
            Console.ResetColor();

            _Logger.Error(messageFormat,args);
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            _Logger.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Concat(message," : ", ex.Message));
            Console.ResetColor();

            _Logger.Error(message,ex);
        }

        public void Error(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();

            _Logger.Error(ex);
        }

        public void FatalError(string messageFormat, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(messageFormat, args);
            Console.ResetColor();

            _Logger.FatalError(messageFormat,args);
        }

        public void FatalError(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            _Logger.FatalError(message);
        }

        public void FatalError(string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            _Logger.FatalError(message,ex);
        }

        public void FatalError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();

            _Logger.FatalError(ex);
        }

        public void Info(string messageFormat, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(messageFormat, args);
            Console.ResetColor();

            _Logger.Info(messageFormat,args);
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();

            _Logger.Info(message);
        }

        public void Warning(string messageFormat, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine(messageFormat, args);
            Console.ResetColor();

            _Logger.Warning(messageFormat,args);
        }

        public void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();

            _Logger.Warning(message);
        }

    }
}
