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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Infection
{
    public class Service : AbstractService
    {
        protected override void Run(DataDimensions dimensions)
        {

            var serviceList = new List<IService>();
            DateTime syncTime = DateTime.Now.ToUniversalTime();

            var infections = _DataContext
                .CreateQuery<Domain.Models.InfectionVerification>()
                .FilterBy(x => x.Room.Wing.Floor.Facility.Id == _Facility.Id)
                .FilterBy(x => x.LastSynchronizedAt == null || x.LastUpdatedAt > x.LastSynchronizedAt)
                .FetchAll().ToList();

            if(infections.Count() < 1)
            {
                return;
            }


            using (var transaction = _DataContext.BeginTransaction())
            {
                _DataContext.AuditTrackingEnabled(false);
                foreach (var infection in infections)
                {
                    infection.LastSynchronizedAt = syncTime;
                    _DataContext.Update(infection);
                }
                _DataContext.AuditTrackingEnabled(true);

                var service = new
                    FactServices.Infection(
                     _DimensionBuilderRepository,
                     _DimensionRepository,
                     _CubeBuilderRepository,
                     _DataContext,
                     _FactBuilderRespository,
                     _Log,
                     _Store);

                service.Run(infections, _Facility, dimensions);

                transaction.Commit();
            }

            AddCubeSyncJob<CubeServices.FacilityMonthInfectionType>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FloorMapRoomInfectionType>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FloorMonthInfectionType>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.WingMonthInfectionSite>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthInfectionSite>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityInfectionSite>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FloorMapRoomInfectionType>(1, dimensions, _ScanStartDate, _Facility.Id);

            AddCubeSyncJob<SynchronizationService.SymptomaticCUATI.CubeServices.FacilityMonthSCAUTI>(1, dimensions, _ScanStartDate, _Facility.Id);

          
        }
    }
}
