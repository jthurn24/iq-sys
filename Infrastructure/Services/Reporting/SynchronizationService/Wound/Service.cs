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
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Wound
{
    public class Service : AbstractService
    {
        protected override void Run(DataDimensions dimensions)
        {

            var serviceList = new List<IService>();
            DateTime syncTime = DateTime.Now.ToUniversalTime();

            var reports = _DataContext
                .CreateQuery<Domain.Models.WoundReport>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == _Facility.Id)
                .FilterBy(x => x.LastSynchronizedAt == null || x.LastUpdatedAt > x.LastSynchronizedAt)
                .FilterBy(x => x.CurrentStage != null )
                .FetchAll().ToList();

            if(reports.Count() < 1)
            {
                return;
            }

            using (var transaction = _DataContext.BeginTransaction())
            {
                _DataContext.AuditTrackingEnabled(false);
                foreach (var report in reports)
                {
                    report.LastSynchronizedAt = syncTime;
                    _DataContext.Update(report);
                }
                _DataContext.AuditTrackingEnabled(true);

                var service = new
                    FactServices.Wound(
                     _DimensionBuilderRepository,
                     _DimensionRepository,
                     _CubeBuilderRepository,
                     _DataContext,
                     _FactBuilderRespository,
                     _Log,
                     _Store);

                service.Run(reports, _Facility, dimensions);

                transaction.Commit();
            }


            AddCubeSyncJob<CubeServices.FacilityMonthWoundClassification>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthWoundSite>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthWoundStage>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.WingMonthWoundType>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FloorMapRoomWoundStage>(1, dimensions, _ScanStartDate, _Facility.Id);

        }
    }
}
