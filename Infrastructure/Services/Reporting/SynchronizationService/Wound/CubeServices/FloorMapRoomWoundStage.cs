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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Wound.CubeServices
{
    public class FloorMapRoomWoundStage : AbstractService
    {
        protected override void Run(DataDimensions changes)
        {
            _Log.Info(string.Format("Syncing cube FloorMapRoomDayWoundStage starting: {0} ending: {1} facility: {2}", changes.StartDate, changes.EndDate, changes.Facility.Name));

            var facts = GetQueryable<Facts.WoundReport>()
                .Where(x => x.Facility.Id == changes.Facility.Id);

            foreach (var floorMap in GetQueryable<Dimensions.FloorMap>()
                .Where(x => x.Facility.Id == changes.Facility.Id && x.Active == true)
                )
            {
                var cube = GetQueryable<Cubes.FloorMapRoomWoundStage>()
                    .Where(x => x.FloorMap.Id == floorMap.Id)
                    .FirstOrDefault();


                if (cube == null)
                {
                    cube = new Cubes.FloorMapRoomWoundStage();

                }

                cube.FloorMap = floorMap;
                cube.RoomEntries = new List<Cubes.FloorMapRoomWoundStage.RoomEntry>();

                foreach (var fact in facts)
                {
                    foreach (var assessment in fact.Assessments
                        .Where(x => x.AssessmentDate.HasValue && x.FloorMap == floorMap))
                    {
                        var roomEntry = cube.RoomEntries.Where(x => x.FloorMapRoom.Id == assessment.FloorMapRoom.Id).FirstOrDefault();

                        if (roomEntry == null)
                        {
                            roomEntry = new Cubes.FloorMapRoomWoundStage.RoomEntry();
                            roomEntry.FloorMapRoom = assessment.FloorMapRoom;
                            roomEntry.EntityEntries = new List<Cubes.FloorMapRoomWoundStage.EntityEntry>();
                            cube.RoomEntries.Add(roomEntry);
                        }

                        var e = new Cubes.FloorMapRoomWoundStage.EntityEntry();
                        e.FloorMapRoom = assessment.FloorMapRoom;
                        e.Component = fact.Id;
                        e.StartDate = assessment.AssessmentDate.Value;
                        e.EndDate = assessment.CoverageEndDate;
                        e.WoundStage = assessment.Stage;

                        roomEntry.EntityEntries.Add(e);
                        
                    }


                }


                Save<Cubes.FloorMapRoomWoundStage>(cube);
            }


        }


    }
}
