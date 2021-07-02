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
namespace IQI.Intuition.Infrastructure.Services.Utilities
{
    public class SyncWoundSites : IConsoleService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private IDimensionRepository _DimensionRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IFactBuilderRepository _FactBuilderRespository;
        private ILog _Log;
        private ISessionActivator _SessionActivator;
        private AuditTrackingWorker _AuditWorker;

        public SyncWoundSites(
                IDimensionBuilderRepository dimensionBuilderRepository,
                IDimensionRepository dimensionRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IStatelessDataContext statelessDataContext,
                IFactBuilderRepository factBuilderRespository,
                ILog log,
                ISessionActivator sessionActivator,
                AuditTrackingWorker auditWorker
            )
        {
            _DimensionBuilderRepository = dimensionBuilderRepository;
            _CubeBuilderRepository = cubeBuilderRepository;
            _FactBuilderRespository = factBuilderRespository;
            _DimensionRepository = dimensionRepository;
            _Log = log;
            _SessionActivator = sessionActivator;
            _AuditWorker = auditWorker;
        }

        public void Run(string[] args)
        {
            using (var dataContext = CreateDataContext())
            {

                foreach (var site in dataContext.CreateQuery<Domain.Models.WoundSite>()
                    .FetchAll())
                {
                    _DimensionBuilderRepository.GetOrCreateWoundSite(site.Name,
                        site.TopLeftX.Value,
                        site.TopLeftY.Value,
                        site.BottomRightX.Value,
                        site.BottomRightY.Value);

                }
            }
        }

        private IStatelessDataContext CreateDataContext()
        {
            return new StatelessDataContext(this._SessionActivator, this._AuditWorker);
        }
    }
}
