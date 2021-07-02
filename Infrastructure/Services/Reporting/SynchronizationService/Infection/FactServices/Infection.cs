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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Infection.FactServices
{
    public class Infection : AbstractFactService
    {
        public Infection(
            IDimensionBuilderRepository db,
            IDimensionRepository d,
            ICubeBuilderRepository cb,
            IStatelessDataContext dc,
            IFactBuilderRepository fb,
            ILog log,
            IDocumentStore ds)
            : base(db, d, cb, dc, fb, log, ds)
        {

        }


        public void Run(
            IList<Domain.Models.InfectionVerification> dInfections,
            Domain.Models.Facility dFacility,
            DataDimensions dimensions)
        {

            foreach (var dInfection in dInfections)
            {

                _Log.Info(string.Format("Syncing infection: {0}  facility: {1}", dInfection.Guid, dFacility.Name));

                Dimensions.Day firstNotedOnDay = null;
                Dimensions.Month firstNotedOnMonth = null;
                Dimensions.Quarter firstNotedOnQuarter = null;

                Dimensions.Day closedOnDay = null;
                Dimensions.Month closedOnMonth = null;
                Dimensions.Quarter closedOnQuarter = null;

                if (dInfection.FirstNotedOn.HasValue)
                {
                    firstNotedOnDay = _DimensionBuilderRepository.GetOrCreateDay(dInfection.FirstNotedOn.Value);
                    firstNotedOnMonth = _DimensionBuilderRepository.GetOrCreateMonth(dInfection.FirstNotedOn.Value.Month, dInfection.FirstNotedOn.Value.Year);
                    firstNotedOnQuarter = firstNotedOnMonth.Quarter;
                }

                if (dInfection.ResolvedOn.HasValue)
                {
                    closedOnDay = _DimensionBuilderRepository.GetOrCreateDay(dInfection.ResolvedOn.Value);
                    closedOnMonth = _DimensionBuilderRepository.GetOrCreateMonth(dInfection.ResolvedOn.Value.Month, dInfection.ResolvedOn.Value.Year);
                    closedOnQuarter = closedOnMonth.Quarter;
                }

                var dPatient = _DataContext.Fetch<Patient>(dInfection.Patient.Id);
                var dRoom = _DataContext.Fetch<Room>(dInfection.Room.Id);
                var dWing = _DataContext.Fetch<Wing>(dRoom.Wing.Id);
                var dFloor = _DataContext.Fetch<Floor>(dWing.Floor.Id);
                var dInfectionSite = _DataContext.Fetch<InfectionSite>(dInfection.InfectionSite.Id);
                var dInfectionType = _DataContext.Fetch<InfectionType>(dInfectionSite.Type.Id);
                var dAccount = _DataContext.Fetch<Domain.Models.Account>(dFacility.Account.Id);

                InfectionSiteSupportingDetail dSupportingDetail = null;

                if (dInfection.InfectionSiteSupportingDetail != null)
                {
                    dSupportingDetail = _DataContext.Fetch<InfectionSiteSupportingDetail>(dInfection.InfectionSiteSupportingDetail.Id);
                }


                var infectionType = _DimensionBuilderRepository.GetOrCreateInfectionType(dInfectionType.Name, dInfectionType.Color, dInfectionType.ShortName);
                var infectionSite = _DimensionBuilderRepository.GetOrCreateInfectionSite(dInfectionSite.Name, infectionType);
                var infectionClassification = _DimensionBuilderRepository.GetOrCreateInfectionClassification(System.Enum.GetName(typeof(Domain.Models.InfectionClassification), dInfection.Classification));
                var room = _DimensionBuilderRepository.GetOrCreateRoom(dRoom.Guid);
                var wing = _DimensionBuilderRepository.GetOrCreateWing(dWing.Guid);
                var floor = _DimensionBuilderRepository.GetOrCreateFloor(dFloor.Guid);
                var account = _DimensionBuilderRepository.GetOrCreateAccount(dAccount.Guid, dAccount.Name);
                var facility = _DimensionBuilderRepository.GetOrCreateFacility(dFacility.Guid);


                Dimensions.FloorMap floorMap;
                
                if(facility.HasSingleFloorMap.HasValue && facility.HasSingleFloorMap.Value == true)
                {
                    floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(facility);
                }
                else
                {
                    floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(wing);
                }

                var floorMapRoom = _DimensionBuilderRepository.GetOrCreateFloorMapRoom(room, floorMap);

                var record = _FactBuilderRespository.GetOrCreateInfectionVerification(dInfection.Guid);

                /* Track changing dimensions before updating records */
                TrackDimensionChanges(dimensions,record);

                record.InfectionType = infectionType;
                record.InfectionClassification = infectionClassification;
                record.InfectionSite = infectionSite;
                record.Room = room;
                record.Wing = wing;
                record.Floor = floor;
                record.Account = account;
                record.Facility = facility;
                record.FloorMap = floorMap;
                record.FloorMapRoom = floorMapRoom;
                record.ClosedOnMonth = closedOnMonth;
                record.ClosedOnQuarter = closedOnQuarter;
                record.ClosedOnDay = closedOnDay;
                record.ClosedOnDate = dInfection.ResolvedOn;
                record.NotedOnMonth = firstNotedOnMonth;
                record.NotedOnQuarter = firstNotedOnQuarter;
                record.NotedOnDay = firstNotedOnDay;
                record.NotedOnDate = dInfection.FirstNotedOn;
                record.Deleted = dInfection.Deleted;
                record.SupportingDetail = dSupportingDetail != null ? dSupportingDetail.Name : string.Empty;

                Save<Facts.InfectionVerification>(record);

                TrackDimensionChanges(dimensions, record);

            }

        }

        private void TrackDimensionChanges(DataDimensions dimensions, Facts.InfectionVerification infection)
        {
            if (infection.InfectionType != null ) {
                if (dimensions.InfectionTypes.Count(x => x.Name == infection.InfectionType.Name) < 1)
                {
                    dimensions.InfectionTypes.Add(infection.InfectionType);
                }
            }


            if (infection.Wing != null)
            {
                if (dimensions.Wings.Count(x => x.Id == infection.Wing.Id) < 1)
                {
                    dimensions.Wings.Add(infection.Wing);
                }
            }

            if (infection.Floor != null)
            {
                if (dimensions.Floors.Count(x => x.Id == infection.Floor.Id) < 1)
                {
                    dimensions.Floors.Add(infection.Floor);
                }
            }

            if (infection.InfectionSite != null)
            {
                if (dimensions.InfectionSites.Count(x => x.Name == infection.InfectionSite.Name) < 1)
                {
                    dimensions.InfectionSites.Add(infection.InfectionSite);
                }
            }

            if (infection.InfectionClassification != null) {
                if (dimensions.InfectionClassifications.Count(x => x.EnumName == infection.InfectionClassification.EnumName) < 1)
                {
                    dimensions.InfectionClassifications.Add(infection.InfectionClassification);
                }
            }

            if (infection.NotedOnDate.HasValue)
            {
                DateTime startDate = infection.NotedOnDate.Value;
                DateTime endDate = DateTime.Today;

                if (infection.ClosedOnDate.HasValue)
                {
                    endDate = infection.ClosedOnDate.Value;
                }

                if (dimensions.StartDate.HasValue == false || dimensions.StartDate.Value > startDate)
                { 
                    dimensions.StartDate = startDate; 
                }

                if (dimensions.EndDate.HasValue == false || dimensions.EndDate.Value < endDate)
                {
                    dimensions.EndDate = endDate;
                }
            }         

        }

    }
}
