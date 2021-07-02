using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using Facts = IQI.Intuition.Reporting.Models.Facts;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using RedArrow.Framework.Utilities;
using IQI.Intuition.Reporting.Containers;
using IQI.Intuition.Reporting.Models.User;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService
{
    public abstract class AbstractService : IService
    {
        protected IDimensionBuilderRepository _DimensionBuilderRepository;
        protected IDimensionRepository _DimensionRepository;
        protected ICubeBuilderRepository _CubeBuilderRepository;
        protected IFactBuilderRepository _FactBuilderRespository;
        protected ILog _Log;
        protected Domain.Models.Facility _Facility;
        protected DateTime _ScanStartDate;
        protected IStatelessDataContext _DataContext;
        protected IDocumentStore _Store;

        public void Run(DateTime scanStartDate,
            Domain.Models.Facility facility,
            DataDimensions dimensions,
            IDimensionBuilderRepository dimensionBuilderRepository,
            IDimensionRepository dimensionRepository,
            ICubeBuilderRepository cubeBuilderRepository,
            IStatelessDataContext dataContext,
            IFactBuilderRepository factBuilderRespository,
            ILog log,
            IDocumentStore store)
        {
            _DimensionBuilderRepository = dimensionBuilderRepository;
            _DimensionRepository = dimensionRepository;
            _CubeBuilderRepository = cubeBuilderRepository;
            _FactBuilderRespository = factBuilderRespository;
            _Log = log;
            _Facility = facility;
            _ScanStartDate = scanStartDate;
            _DataContext = dataContext;
            _Store = store;

            Run(dimensions);
        }

        abstract protected void Run(DataDimensions dimensions);


        protected void AddCubeSyncJob<T>(int priority, 
            DataDimensions data,
            DateTime startScanDate,
            int facilityId) where T : AbstractService
        {
            var job = new CubeSyncJob();
            job.Id = GuidHelper.NewGuid();
            job.Dimensions = data;
            job.CreatedOn = DateTime.Now;
            job.Priority = priority;
            job.ServiceTypeName = typeof(T).AssemblyQualifiedName;
            job.ScanStartDate = startScanDate;
            job.FacilityId = facilityId;
            Insert(job);

            _Log.Info("Queued Cubesync Job {0} Pri: {1}", job.Id, job.Priority);
        }


        protected IQueryable<T> GetQueryable<T>()
        {
            return _Store.GetQueryable<T>();
        }

        protected void Insert<T>(T obj)
        {
            _Store.Insert<T>(obj);
        }

        protected void Save<T>(T obj)
        {
            _Store.Save<T>(obj);
        }

        protected void LogInfo(string message)
        {
            _Log.Info(message);
            System.Console.WriteLine(message);

        }
    }
}
