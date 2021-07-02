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
    public class IncrementalService : IConsoleService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private IDimensionRepository _DimensionRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IFactBuilderRepository _FactBuilderRespository;
        private ILog _Log;
        private ISessionActivator _SessionActivator;
        private AuditTrackingWorker _AuditWorker;
        private IDocumentStore _Store;

        public IncrementalService(
                IDimensionBuilderRepository dimensionBuilderRepository,
                IDimensionRepository dimensionRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IStatelessDataContext statelessDataContext,
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
            _Log = log;
            _SessionActivator = sessionActivator;
            _AuditWorker = auditWorker;
            _Store = store;
        }

        public void Run(string[] args)
        {
            using (var dataContext = CreateDataContext())
            {
                var facilityBatch =  CreateDataContext().CreateQuery<Domain.Models.Facility>()
                    .FetchAll()
                    .OrderBy(x => x.LastSynchronizedAt)
                    .Take(15);

                //_Log.Info("Scanning {0}", facilityBatch.Select(x => x.Name).ToDelimitedString(','));

                foreach (var facility in facilityBatch)
                {
              
                    #if (DEBUG)
                        Process(facility, dataContext);
                    #else
                        try
                        {
                            Process(facility, dataContext);
                        }
                        catch (Exception ex)
                        {
                            _Log.Error(ex);
                        }
                    #endif


                }
            }
        }

        private void Process(Domain.Models.Facility facility, IStatelessDataContext dataContext)
        {
            var serviceList = new List<IService>();
            
            if (facility.LastSynchronizedAt.HasValue == false)
            {
                facility.LastSynchronizedAt = new DateTime(2000, 1, 1);
                SetFacilityUpdateTime(facility.Id, new DateTime(2000, 1, 1));
            }

            /* Use this time, as items may be updated while this process is running */
            var facilityUpdateTime = DateTime.Now.ToUniversalTime();
      
            serviceList.Add(new Facility.Service());
            serviceList.Add(new PatientCensus.Service());
            serviceList.Add(new Infection.Service());
            serviceList.Add(new Incident.Service());
            serviceList.Add(new Psychotropic.Service());
            serviceList.Add(new Wound.Service());
            serviceList.Add(new Complaint.Service());
            serviceList.Add(new Catheter.Service());


            /* Start Sync */


            foreach (var service in serviceList)
            {
                var dimensions = new DataDimensions();
                dimensions.Facility = _DimensionBuilderRepository.GetOrCreateFacility(facility.Guid);

                service.Run(facility.LastSynchronizedAt.Value,
                    facility,
                    dimensions,
                    _DimensionBuilderRepository,
                    _DimensionRepository,
                    _CubeBuilderRepository,
                    dataContext,
                    _FactBuilderRespository,
                    _Log,
                    _Store);
            }


            SetFacilityUpdateTime(facility.Id, facilityUpdateTime);

        }

        private IStatelessDataContext CreateDataContext()
        {
            return new StatelessDataContext(this._SessionActivator, this._AuditWorker);
        }

        private void SetFacilityUpdateTime(int id, DateTime time)
        {
            var transactionSession = _SessionActivator.CreateSession(string.Empty);
            transactionSession.BeginTransaction();
            var tFacility = transactionSession.Get<Domain.Models.Facility>(id);
            tFacility.LastSynchronizedAt = time;
            transactionSession.Transaction.Commit();
            transactionSession.Flush();
            transactionSession.Dispose();
        }
    }
}
