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
using SnyderIS.sCore.Persistence;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using RedArrow.Framework.ObjectModel.AuditTracking;
using IQI.Intuition.Reporting.Containers;


namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService
{
    public class CubeJobService : IConsoleService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private IDimensionRepository _DimensionRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IFactBuilderRepository _FactBuilderRespository;
        private IUserRepository _UserRepository;
        private ILog _Log;
        private ISessionActivator _SessionActivator;
        private AuditTrackingWorker _AuditWorker;
        private IDocumentStore _Store;

        private static IDictionary<string, Type> _ServiceTypeMap;

        public CubeJobService(
                IDimensionBuilderRepository dimensionBuilderRepository,
                IDimensionRepository dimensionRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IStatelessDataContext statelessDataContext,
                IFactBuilderRepository factBuilderRespository,
                ILog log,
                ISessionActivator sessionActivator,
                AuditTrackingWorker auditWorker,
                IDocumentStore store,
                IUserRepository userRepository
            )
        {
            _DimensionBuilderRepository = dimensionBuilderRepository;
            _CubeBuilderRepository = cubeBuilderRepository;
            _FactBuilderRespository = factBuilderRespository;
            _DimensionRepository = dimensionRepository;
            _Log = log;
            _SessionActivator = sessionActivator;
            _AuditWorker = auditWorker;
            _Store = store;
            _UserRepository = userRepository;
            _ServiceTypeMap = new Dictionary<string, Type>();
        }

        public void Run(string[] args)
        {
            using (var dataContext = CreateDataContext())
            {

                var nextJob = _UserRepository.GetNextCubeSyncJob();

                if (nextJob != null)
                {
                    _Log.Info("Processing 1 of {0} jobs ", _UserRepository.GetCubeSyncJobCount());

                    if (_ServiceTypeMap.ContainsKey(nextJob.ServiceTypeName) == false)
                    {
                        _ServiceTypeMap[nextJob.ServiceTypeName] = Type.GetType(nextJob.ServiceTypeName);
                    }


                    try
                    {
                        var startTime = DateTime.Now;
                        var service = (AbstractService)Activator.CreateInstance(_ServiceTypeMap[nextJob.ServiceTypeName]);

                        var facility = dataContext.CreateQuery<Domain.Models.Facility>()
                            .FilterBy(x => x.Id == nextJob.FacilityId)
                            .FetchFirst();

                        service.Run(nextJob.ScanStartDate,
                            facility,
                            nextJob.Dimensions,
                            _DimensionBuilderRepository,
                            _DimensionRepository,
                            _CubeBuilderRepository,
                            dataContext,
                            _FactBuilderRespository,
                            _Log,
                            _Store);

                        var endTime = DateTime.Now;

                        _Log.Info("Finished Cubesync Job {0}:{1}  Facility: {2}  Priority: {3} Scan: {4} Seconds: {5}  ",
                            nextJob.Id,
                            _ServiceTypeMap[nextJob.ServiceTypeName].Name,
                            facility.Name,
                            nextJob.Priority,
                            nextJob.ScanStartDate,
                            endTime.Subtract(startTime).TotalSeconds);

                        _UserRepository.Delete(nextJob);
                        

                    }
                    catch (Exception ex)
                    {
                        /* If an error occurred we want to let it reprocess again.. but we lower the priority so that it does
                         * not potentially impact other jobs (if this job continues to fail)
                        */

                        nextJob.Priority = 50;
                        nextJob.CreatedOn = DateTime.Now;
                        _UserRepository.Update(nextJob);

                        _Log.Error(ex);
                    }

                }



            }
        }

        private IStatelessDataContext CreateDataContext()
        {
            return new StatelessDataContext(this._SessionActivator, this._AuditWorker);
        }


    }
}
