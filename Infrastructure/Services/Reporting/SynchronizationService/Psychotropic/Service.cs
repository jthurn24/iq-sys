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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Psychotropic
{
    public class Service : AbstractService
    {
        protected override void Run(DataDimensions dimensions)
        {
            var serviceList = new List<IService>();
            DateTime syncTime = DateTime.Now.ToUniversalTime();

            var administrations = _DataContext
                .CreateQuery<Domain.Models.PsychotropicAdministration>()
                .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == _Facility.Id)
                .FilterBy(x => x.LastSynchronizedAt == null || x.LastUpdatedAt > x.LastSynchronizedAt)
                .FetchAll().ToList();

            if(administrations.Count() < 1)
            {
                return;
            }

            using (var transaction = _DataContext.BeginTransaction())
            {
                _DataContext.AuditTrackingEnabled(false);
                foreach (var admin in administrations)
                {
                    admin.LastSynchronizedAt = syncTime;
                    _DataContext.Update(admin);
                }
                _DataContext.AuditTrackingEnabled(true);

                var service = new
                    FactServices.PsychotropicAdministration(
                     _DimensionBuilderRepository,
                     _DimensionRepository,
                     _CubeBuilderRepository,
                     _DataContext,
                     _FactBuilderRespository,
                     _Log,
                     _Store);

                service.Run(administrations, _Facility, dimensions);

                transaction.Commit();
            }


            AddCubeSyncJob<CubeServices.FacilityMonthPsychotropicDrugType>(1, dimensions, _ScanStartDate, _Facility.Id);

        }
    }
}
