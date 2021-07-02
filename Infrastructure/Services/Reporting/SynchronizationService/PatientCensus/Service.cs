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
using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.PatientCensus
{
    public class Service : AbstractService
    {

        protected override void Run(DataDimensions dimensions)
        {

            var facilityDimension = _DimensionBuilderRepository.GetOrCreateFacility(_Facility.Guid);
            DateTime syncTime = DateTime.Now.ToUniversalTime();


            var censusRecords = _DataContext.CreateQuery<Domain.Models.PatientCensus>()
                .FilterBy(x => x.Facility.Id == _Facility.Id)
                .FilterBy(x => x.LastSynchronizedAt == null || x.LastUpdatedAt > x.LastSynchronizedAt)
                .FilterBy( x=> x.PatientDays > 0)
                .FetchAll();

            if(censusRecords.Count() < 1)
            {
                return;
            }

            foreach (var census in censusRecords)
            {


                var censusStartDate = new DateTime(census.Year, census.Month, 1);
                var censusNextStartDate = censusStartDate.AddMonths(1);

                var month = _DimensionBuilderRepository.GetOrCreateMonth(census.Month, census.Year);


                var cService = new SynchronizationService.PatientCensus.CubeServices.FacilityMonthCensus(
                    _DimensionBuilderRepository,
                    _CubeBuilderRepository,
                    _DataContext,
                    _FactBuilderRespository,
                    _Log,
                    _Store);

                cService.Run(facilityDimension, month);


                /* A census change impacts pretty much every dimension in the systems that is tied to a date and does
                 * a ##/1000 calc. We need to let the dimension tracker know that cube services must re-eval everything */

                dimensions.StartDate = censusStartDate;

                dimensions.EndDate = censusNextStartDate;

                _Log.Info("Census change induced facility sync {0} - {1} - {2}", facilityDimension.Name, censusStartDate, censusNextStartDate);

                dimensions.Wings.AddRange(_DimensionRepository.GetWingsForFacility(facilityDimension.Id));
                dimensions.Floors.AddRange(_DimensionRepository.GetFloorsForFacility(facilityDimension.Id));

                dimensions.InfectionClassifications.AddRange(_DimensionRepository.GetInfectionClassifications().ToList());
                dimensions.InfectionTypes.AddRange(_DimensionRepository.GetInfectionTypes().ToList());
                dimensions.InfectionSites.AddRange(_DimensionRepository.GetInfectionSites().ToList());

                dimensions.IncidentInjuries.AddRange(_DimensionRepository.GetIncidentInjuries().ToList());
                dimensions.IncidentInjuryLevels.AddRange(_DimensionRepository.GetIncidentInjuryLevels().ToList());
                dimensions.IncidentLocations.AddRange(_DimensionRepository.GetIncidentLocations().ToList());
                dimensions.IncidentTypeGroups.AddRange(_DimensionRepository.GetIncidentTypeGroups().ToList());
                dimensions.IncidentTypes.AddRange(_DimensionRepository.GetIncidentTypes().ToList());

                dimensions.PsychotropicDrugTypes.AddRange(_DimensionRepository.GetAllPsychotropicDrugTypes().ToList());

                dimensions.WoundClassifications.AddRange(_DimensionRepository.GetWoundClassifications().ToList());
                dimensions.WoundSites.AddRange(_DimensionRepository.GetWoundSites().ToList());
                dimensions.WoundTypes.AddRange(_DimensionRepository.GetWoundTypes().ToList());
                dimensions.WoundStages.AddRange(_DimensionRepository.GetWoundStages().ToList());

                
                AddCubeSyncJob<Infection.CubeServices.FacilityMonthInfectionType>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FloorMapRoomInfectionType>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FloorMonthInfectionType>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Infection.CubeServices.WingMonthInfectionSite>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FacilityMonthInfectionSite>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FacilityInfectionSite>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FloorMapRoomInfectionType>(1, dimensions, _ScanStartDate, _Facility.Id);

                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentType>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentTypeGroup>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentInjury>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentInjuryLevel>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentLocation>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentDayOfWeek>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentHourOfDay>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FloorMonthIncidentTypeGroup>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.WingMonthIncidentTypeGroup>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentType>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FloorMapRoomIncidentType>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FloorMapRoomIncidentInjury>(1, dimensions, _ScanStartDate, _Facility.Id);

                AddCubeSyncJob<Catheter.CubeServices.FacilityMonthCatheter>(1, dimensions, _ScanStartDate, _Facility.Id);

                AddCubeSyncJob<Complaint.CubeServices.FacilityMonthComplaintType>(5, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Complaint.CubeServices.WingMonthComplaintType>(5, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Complaint.CubeServices.FloorMonthComplaintType>(5, dimensions, _ScanStartDate, _Facility.Id);

                AddCubeSyncJob<Psychotropic.CubeServices.FacilityMonthPsychotropicDrugType>(1, dimensions, _ScanStartDate, _Facility.Id);

                AddCubeSyncJob<Wound.CubeServices.FacilityMonthWoundClassification>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Wound.CubeServices.FacilityMonthWoundSite>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Wound.CubeServices.FacilityMonthWoundStage>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Wound.CubeServices.WingMonthWoundType>(1, dimensions, _ScanStartDate, _Facility.Id);
                AddCubeSyncJob<Wound.CubeServices.FloorMapRoomWoundStage>(1, dimensions, _ScanStartDate, _Facility.Id);

                AddCubeSyncJob<SymptomaticCUATI.CubeServices.FacilityMonthSCAUTI>(1, dimensions, _ScanStartDate, _Facility.Id);


                using (var transaction = _DataContext.BeginTransaction())
                {
                    _DataContext.AuditTrackingEnabled(false);
                    foreach (var c in censusRecords)
                    {
                        c.LastSynchronizedAt = syncTime;
                        _DataContext.Update(c);
                    }
                    _DataContext.AuditTrackingEnabled(true);

                    transaction.Commit();
                }


            }
        
        }
    }
}
