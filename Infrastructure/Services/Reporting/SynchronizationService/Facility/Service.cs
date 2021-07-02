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
using IQI.Intuition.Reporting.Containers;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Utilities;
using IQI.Intuition.Reporting.Models.User;


namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Facility
{
    public class Service : IService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private IDimensionRepository _DimensionRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IStatelessDataContext _DataContext;
        private IFactBuilderRepository _FactBuilderRespository;
        private ILog _Log;
        private IDocumentStore _Store;

        public Service()
        {
        }

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
            _DataContext = dataContext;
            _Log = log;
            _Store = store;

            var account = _DataContext.Fetch<Domain.Models.Account>(facility.Account.Id);

            var accountDimension = _DimensionBuilderRepository.GetOrCreateAccount(account.Guid, account.Name);
            accountDimension.Name = account.Name;
            Save<Dimensions.Account>(accountDimension);

            var facilityDimension = _DimensionBuilderRepository.GetOrCreateFacility(facility.Guid);
            facilityDimension.Account = accountDimension;
            facilityDimension.Name = facility.Name;
            facilityDimension.HasSingleFloorMap = facility.HasSingleFloorMap;
            Save<Dimensions.Facility>(facilityDimension);

            var floors = _DataContext.CreateQuery<Domain.Models.Floor>()
                .FilterBy(x => x.Facility.Id == facility.Id)
                .FilterBy(x => x.LastUpdatedAt >= scanStartDate)
                .FetchAll();

            SyncFloors(facilityDimension,floors);

            var wings = _DataContext.CreateQuery<Domain.Models.Wing>()
                .FilterBy(x => x.Floor.Facility.Id == facility.Id)
                .FilterBy(x => x.LastUpdatedAt >= scanStartDate)
                .FetchAll();

            SyncWings(facilityDimension, wings);

            var rooms = _DataContext.CreateQuery<Domain.Models.Room>()
                .FilterBy(x => x.Wing.Floor.Facility.Id == facility.Id)
                .FilterBy(x => x.LastUpdatedAt >= scanStartDate)
                .FetchAll();

            SyncRooms(facilityDimension, rooms);

            /* If anything changed trigger a full facility sync */
            if(floors.Count() > 0 || wings.Count() > 0 || rooms.Count() > 0)
            {
                dimensions.StartDate = new DateTime(2011,1,1);

                dimensions.EndDate = DateTime.Today;

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


                AddCubeSyncJob<Infection.CubeServices.FacilityMonthInfectionType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FloorMapRoomInfectionType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FloorMonthInfectionType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Infection.CubeServices.WingMonthInfectionSite>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FacilityMonthInfectionSite>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FacilityInfectionSite>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Infection.CubeServices.FloorMapRoomInfectionType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);

                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentTypeGroup>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentInjury>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentInjuryLevel>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentLocation>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentDayOfWeek>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentHourOfDay>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FloorMonthIncidentTypeGroup>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.WingMonthIncidentTypeGroup>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FacilityMonthIncidentType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FloorMapRoomIncidentType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Incident.CubeServices.FloorMapRoomIncidentInjury>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);

                AddCubeSyncJob<Catheter.CubeServices.FacilityMonthCatheter>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);

                AddCubeSyncJob<Complaint.CubeServices.FacilityMonthComplaintType>(5, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Complaint.CubeServices.WingMonthComplaintType>(5, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Complaint.CubeServices.FloorMonthComplaintType>(5, dimensions, new DateTime(2011, 1, 1), facility.Id);

                AddCubeSyncJob<Psychotropic.CubeServices.FacilityMonthPsychotropicDrugType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);

                AddCubeSyncJob<Wound.CubeServices.FacilityMonthWoundClassification>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Wound.CubeServices.FacilityMonthWoundSite>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Wound.CubeServices.FacilityMonthWoundStage>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Wound.CubeServices.WingMonthWoundType>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);
                AddCubeSyncJob<Wound.CubeServices.FloorMapRoomWoundStage>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);

                AddCubeSyncJob<SymptomaticCUATI.CubeServices.FacilityMonthSCAUTI>(1, dimensions, new DateTime(2011, 1, 1), facility.Id);

            }

        }



        private void SyncFloors(Dimensions.Facility facilityDimension,
            IEnumerable<Domain.Models.Floor> floors)
        {
            foreach (var floorDomain in floors)
            {
                _Log.Info(string.Format("Syncing floor: {0}  facility: {1}", floorDomain.Name, facilityDimension.Name));
                var floorDimension = _DimensionBuilderRepository.GetOrCreateFloor(floorDomain.Guid);
                floorDimension.Account = facilityDimension.Account;
                floorDimension.Facility = facilityDimension;
                floorDimension.Name = floorDomain.Name;
                Save<Dimensions.Floor>(floorDimension);
            }
        }


        private void SyncWings(Dimensions.Facility facilityDimension,
            IEnumerable<Domain.Models.Wing> wings)
        {
            foreach (var wingDomain in wings)
            {
                var floorDomain = _DataContext.Fetch<Domain.Models.Floor>(wingDomain.Floor.Id);
                var floorDimension = _DimensionBuilderRepository.GetOrCreateFloor(floorDomain.Guid);

                _Log.Info(string.Format("Syncing wing: {0}  facility: {1}", wingDomain.Name, facilityDimension.Name));
                var wingDimension = _DimensionBuilderRepository.GetOrCreateWing(wingDomain.Guid);
                wingDimension.Account = floorDimension.Account;
                wingDimension.Facility = floorDimension.Facility;
                wingDimension.Floor = floorDimension;
                wingDimension.Name = wingDomain.Name;
                Save<Dimensions.Wing>(wingDimension);

                Dimensions.FloorMap floorMap = null;

                if (facilityDimension.HasSingleFloorMap.HasValue && facilityDimension.HasSingleFloorMap.Value == true)
                {
                    floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(facilityDimension);
                    floorMap.Name = facilityDimension.Name;
                    floorMap.Facility = facilityDimension;
                    Save<Dimensions.FloorMap>(floorMap);

                    System.Console.WriteLine("Syncing facility floormap {0}", floorMap.Id);
                }
                else
                {
                    floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(wingDimension);
                    floorMap.Name = string.Concat(floorDimension.Name, " - ", wingDimension.Name);
                    floorMap.Facility = facilityDimension;
                    Save<Dimensions.FloorMap>(floorMap);

                    System.Console.WriteLine("Syncing wing {0}", floorMap.Id);
                }

            }
        }


        private void SyncRooms(Dimensions.Facility facilityDimension,
        IEnumerable<Domain.Models.Room> rooms)
        {
            foreach (var roomDomain in rooms)
            {

                var wingDomain = _DataContext.Fetch<Domain.Models.Wing>(roomDomain.Wing.Id);
                var wingDimension = _DimensionBuilderRepository.GetOrCreateWing(wingDomain.Guid);

                _Log.Info(string.Format("Syncing room: {0}  facility: {1}", roomDomain.Name, facilityDimension.Name));
                var roomDimension = _DimensionBuilderRepository.GetOrCreateRoom(roomDomain.Guid);
                roomDimension.Account = wingDimension.Account;
                roomDimension.Facility = wingDimension.Facility;
                roomDimension.Floor = wingDimension.Floor;
                roomDimension.Wing = wingDimension;
                roomDimension.Name = roomDomain.Name;
                Save<Dimensions.Room>(roomDimension);

                Dimensions.FloorMap floorMap = null;

                if (facilityDimension.HasSingleFloorMap.HasValue && facilityDimension.HasSingleFloorMap.Value == true)
                {
                    floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(facilityDimension);
                }
                else
                {
                    floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(wingDimension);
                }

                var mapRoom = _DimensionBuilderRepository.GetOrCreateFloorMapRoom(roomDimension, floorMap);
                mapRoom.FloorMap = floorMap;

                if (mapRoom.Coordinates == null || mapRoom.Coordinates == string.Empty)
                {
                    mapRoom.Coordinates = "0,0";
                }

                Save<Dimensions.FloorMapRoom>(mapRoom);
            }
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

    }
}
