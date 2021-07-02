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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Incident
{
    public class Service : AbstractService
    {

        protected override void Run(DataDimensions dimensions)
        {
            var serviceList = new List<IService>();
            DateTime syncTime = DateTime.Now.ToUniversalTime();

            var incidents = _DataContext
               .CreateQuery<Domain.Models.IncidentReport>()
               .FilterBy(x => x.Room.Wing.Floor.Facility.Id == _Facility.Id)
               .FilterBy(x => x.LastSynchronizedAt == null || x.LastUpdatedAt > x.LastSynchronizedAt)
               .FetchAll().ToList();

            if(incidents.Count() < 1)
            {
                return;
            }

            using (var transaction = _DataContext.BeginTransaction())
            {

                _DataContext.AuditTrackingEnabled(false);
                foreach (var incident in incidents)
                {
                    incident.LastSynchronizedAt = syncTime;
                    _DataContext.Update(incident);
                }
                _DataContext.AuditTrackingEnabled(true);


                var service = new
                    FactServices.Incident(
                     _DimensionBuilderRepository,
                     _DimensionRepository,
                     _CubeBuilderRepository,
                     _DataContext,
                     _FactBuilderRespository,
                     _Log,
                     _Store);

                service.Run(incidents, _Facility, dimensions);

                transaction.Commit();
            }

            AddCubeSyncJob<CubeServices.FacilityMonthIncidentType>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthIncidentTypeGroup>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthIncidentInjury>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthIncidentInjuryLevel>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthIncidentLocation>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthIncidentDayOfWeek>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthIncidentHourOfDay>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FloorMonthIncidentTypeGroup>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.WingMonthIncidentTypeGroup>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FacilityMonthIncidentType>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FloorMapRoomIncidentType>(1, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FloorMapRoomIncidentInjury>(1, dimensions, _ScanStartDate, _Facility.Id);


        }


    }
}
