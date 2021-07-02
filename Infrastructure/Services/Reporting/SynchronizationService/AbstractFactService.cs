using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Repositories;
using IQI.Intuition.Domain.Models;
using IQI.Intuition.Reporting.Containers;
using IQI.Intuition.Reporting.Models.User;
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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService
{
    public abstract class AbstractFactService
    {
        protected IDimensionBuilderRepository _DimensionBuilderRepository;
        protected IDimensionRepository _DimensionRepository;
        protected ICubeBuilderRepository _CubeBuilderRepository;
        protected IStatelessDataContext _DataContext;
        protected IFactBuilderRepository _FactBuilderRespository;
        protected ILog _Log;
        protected IDocumentStore _Store;


        public AbstractFactService(
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
            _DataContext = dataContext;
            _Log = log;
            _Store = store;

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
    }
}
