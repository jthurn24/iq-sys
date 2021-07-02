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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Complaint
{
    public class Service : AbstractService
    {
        protected override void Run(DataDimensions dimensions)
        {

            var serviceList = new List<IService>();
            DateTime syncTime = DateTime.Now.ToUniversalTime();

            var complaints = _DataContext
                .CreateQuery<Domain.Models.Complaint>()
                .FilterBy(x => x.Facility.Id == _Facility.Id)
                .FilterBy(x => x.LastSynchronizedAt == null || x.LastUpdatedAt > x.LastSynchronizedAt)
                .FetchAll().ToList();

            /* because we are reading without transaction locks, do not process records that have been updated within the last few seconds. We need to give them some time to commit 
            * all changes (such as child relationships */
            complaints = complaints.Where(x => x.LastUpdatedAt == null || x.LastUpdatedAt < DateTime.Now.AddSeconds(-60)).ToList();

            if (complaints.Count() < 1)
            {
                return;
            }

            using (var transaction = _DataContext.BeginTransaction())
            {
                _DataContext.AuditTrackingEnabled(false);
                foreach (var complaint in complaints)
                {
                    complaint.LastSynchronizedAt = syncTime;
                    _DataContext.Update(complaint);
                }
                _DataContext.AuditTrackingEnabled(true);

                var service = new
                    FactServices.Complaint(
                     _DimensionBuilderRepository,
                     _DimensionRepository,
                     _CubeBuilderRepository,
                     _DataContext,
                     _FactBuilderRespository,
                     _Log,
                     _Store);

                service.Run(complaints, _Facility, dimensions);

                transaction.Commit();
            }



            AddCubeSyncJob<CubeServices.FacilityMonthComplaintType>(5, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.WingMonthComplaintType>(5, dimensions, _ScanStartDate, _Facility.Id);
            AddCubeSyncJob<CubeServices.FloorMonthComplaintType>(5, dimensions, _ScanStartDate, _Facility.Id);

          

        }
    }
}
