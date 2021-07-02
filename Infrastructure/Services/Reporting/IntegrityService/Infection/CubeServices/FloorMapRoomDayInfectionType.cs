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

namespace IQI.Intuition.Infrastructure.Services.Reporting.IntegrityService.Infection.CubeServices
{
    public class FloorMapRoomDayInfectionType : IScanService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IStatelessDataContext _DataContext;
        private IFactBuilderRepository _FactBuilderRespository;
        private ConsoleLogWrapper _Log;
        private IDocumentStore _Store;

        public FloorMapRoomDayInfectionType(
                IDimensionBuilderRepository dimensionBuilderRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IStatelessDataContext dataContext,
                IFactBuilderRepository factBuilderRespository,
                ConsoleLogWrapper log,
                IDocumentStore store
            )
        {
            _DimensionBuilderRepository = dimensionBuilderRepository;
            _CubeBuilderRepository = cubeBuilderRepository;
            _FactBuilderRespository = factBuilderRespository;
            _DataContext = dataContext;
            _Log = log;
            _Store = store;
        }

        public void Run(
            Domain.Models.Facility dfacility,
            IQI.Intuition.Reporting.Models.Dimensions.Facility rFacility,
            IList<VarianceDetails> variances,
            int scanDays)
        {
            var cubes = _Store.GetQueryable<Cubes.FloorMapRoomInfectionType>()
            .Where(x => x.FloorMap.Facility.Id == rFacility.Id)
            .ToList();

            var days = _Store.GetQueryable<Dimensions.Day>().Where(x => x.Year > (DateTime.Today.Year - 2)).ToList();
            var types = _Store.GetQueryable<Dimensions.InfectionType>().ToList();
            var classifications = _Store.GetQueryable<Dimensions.InfectionClassification>().ToList();

            var floorMapRooms = _Store.GetQueryable<Dimensions.FloorMapRoom>()
                .Where(x => x.FloorMap.Facility.Id == rFacility.Id)
                .ToList();

            var allDomainInfections = _DataContext.CreateQuery<InfectionVerification>()
                 .FilterBy(x => x.Patient.Room.Wing.Floor.Facility.Id == dfacility.Id)
                 .FilterBy(x => x.Deleted == null || x.Deleted == false)
                 .FilterBy(x => x.Patient.Deleted == null || x.Patient.Deleted == false)
                 .FetchAll();

            var allRooms = _DataContext.CreateQuery<Room>()
                .FilterBy(x => x.Wing.Floor.Facility.Id == dfacility.Id)
                .FetchAll();

            var allInfectionTypes = _DataContext.CreateQuery<InfectionType>()
                .FetchAll();

            DateTime evalDate = DateTime.Today.AddDays(0 - scanDays);


            while (evalDate <= DateTime.Today)
            {


                var day = days.Where(
                    x => x.DayOfMonth == evalDate.Day
                        && x.Year == evalDate.Year
                        && x.MonthOfYear == evalDate.Month)
                        .FirstOrDefault();


                foreach (var type in types)
                {
                    foreach (var floorMapRoom in floorMapRooms)
                    {
                        _Log.SetStatus(string.Format("Inspecting {0} Room {1} Type {2}", evalDate, floorMapRoom.Room.Name, type.Name));

                        foreach (var classification in classifications)
                        {

                            Cubes.FloorMapRoomInfectionType cube = null;



                            bool reportFlag = false;

                            if (day != null)
                            {


                                cube = cubes.Where(x => x.FloorMap.Id == floorMapRoom.FloorMap.Id).FirstOrDefault();

                                /*cube = cubes.Where(
                                    x => x.FloorMapRoom.Id == floorMapRoom.Id
                                        && x.Day.Id == day.Id
                                        && x.InfectionType.Id == type.Id
                                        && x.InfectionClassification.Id == classification.Id)
                                        .FirstOrDefault();
                                */


                                if (cube == null)
                                {
                                    reportFlag = true;
                                }
                                else
                                {
                                    var re = cube.RoomEntries.Where(x => x.FloorMapRoom.Id == floorMapRoom.Id).FirstOrDefault();

                                    if (re == null)
                                    {
                                        reportFlag = true;
                                    }
                                    else
                                    {
                                        var ie = re.EntityEntries.Where(x => x.StartDate >= evalDate &&
                                            (x.EndDate.HasValue == false || x.EndDate >= evalDate)
                                             && x.InfectionType.Id == type.Id
                                            && x.InfectionClassification.Id == classification.Id).FirstOrDefault();

                                        if (ie == null)
                                        {
                                            reportFlag = true;
                                        }
                                    }
                                
                                }
                            }

                            
                            var reportingRoom = floorMapRoom.Room;
                            var domainClassification = (Domain.Models.InfectionClassification)System.Enum.Parse(typeof(Domain.Models.InfectionClassification),classification.EnumName);

                            var room = allRooms.Where(x => x.Guid == reportingRoom.Id).First();
                            var iType = allInfectionTypes.Where(x => x.Name == type.Name).First();

                            var domainInfections = allDomainInfections.Where(x => x.Room.Id == room.Id)
                                             .Where(x => x.InfectionSite.Id == iType.Id)
                                            .Where(x => x.FirstNotedOn <= evalDate)
                                            .Where(x => x.ResolvedOn == null || x.ResolvedOn >= evalDate);

                            bool domainFlag = false;

                            if (domainInfections.Count() > 0)
                            {
                                domainFlag = true;
                            }

                            if (domainFlag != reportFlag)
                            {
                                var dummyReportEntity = new {
                                    FloorMapRoomId = floorMapRoom.Id,
                                    InfectionTypeId = type.Id,
                                    InfectionTypeName = type.Name,
                                    ClassificationName = classification.EnumName,
                                    ClassificationId = classification.Id,
                                    EvalDate = evalDate.ToString("MM/dd/yyyy"),
                                    RoomId = reportingRoom.Id,
                                    RoomName = reportingRoom.Name,
                                    RoomWingId = reportingRoom.Wing.Id,
                                    CubeId = cube != null ? cube.Id : Guid.Empty
                                };

                                var variance = new VarianceDetails();
                                variance.SetReportingEntity(dummyReportEntity);
                                variance.AddDomainEntities(domainInfections);
                                variances.Add(variance);
                            }
    
                            
                        }
                    }
                }


                evalDate = evalDate.AddDays(1);
            }

            _Log.ClearStatus();

        }
    }
}
