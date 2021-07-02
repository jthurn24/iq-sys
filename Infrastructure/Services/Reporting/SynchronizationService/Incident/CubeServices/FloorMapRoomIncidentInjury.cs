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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Incident.CubeServices
{
    public class FloorMapRoomIncidentInjury : AbstractService
    {

        protected override void Run(DataDimensions changes)
        {
            _Log.Info(string.Format("Syncing cube FacilityMapRoomMonthInfectionType starting: {0} ending: {1} facility: {2}", changes.StartDate, changes.EndDate, changes.Facility.Name));

            var facts = GetQueryable<Facts.IncidentReport>()
                .Where(x => x.Facility.Id == changes.Facility.Id)
                .Where(x => x.Deleted == false || x.Deleted == null);

            foreach (var floorMap in GetQueryable<Dimensions.FloorMap>()
                .Where(x => x.Facility.Id == changes.Facility.Id && x.Active == true)
                )
            {
                var cube = GetQueryable<Cubes.FloorMapRoomIncidentInjury>()
                    .Where(x => x.FloorMap.Id == floorMap.Id)
                    .FirstOrDefault();

                var floorMapRooms = GetQueryable<Dimensions.FloorMapRoom>()
                    .Where(x => x.FloorMap.Id == floorMap.Id);

                if (cube == null)
                {
                    cube = new Cubes.FloorMapRoomIncidentInjury();

                }

                cube.FloorMap = floorMap;
                cube.RoomEntries = new List<Cubes.FloorMapRoomIncidentInjury.RoomEntry>();

                foreach (var floorMapRoom in floorMapRooms)
                {
                    var roomFacts = facts.Where(x => x.Room.Id == floorMapRoom.Room.Id);

                    ProcessRoom(floorMapRoom.Room, floorMapRoom, floorMap, cube, roomFacts);
                }

                Save<Cubes.FloorMapRoomIncidentInjury>(cube);
            }


        }

        private void ProcessRoom(Dimensions.Room room,
            Dimensions.FloorMapRoom floorMapRoom,
            Dimensions.FloorMap floorMap,
            Cubes.FloorMapRoomIncidentInjury cube,
            IEnumerable<Facts.IncidentReport> facts)
        {
            var roomEntry = cube.RoomEntries.Where(x => x.FloorMapRoom.Id == floorMapRoom.Id)
                .FirstOrDefault();

            if (roomEntry == null)
            {
                roomEntry = new Cubes.FloorMapRoomIncidentInjury.RoomEntry();
                cube.RoomEntries.Add(roomEntry);
            }

            roomEntry.FloorMapRoom = floorMapRoom;
            roomEntry.EntityEntries = new List<Cubes.FloorMapRoomIncidentInjury.EntityEntry>();

            foreach (var fact in facts)
            {
                roomEntry.EntityEntries.Add(new Cubes.FloorMapRoomIncidentInjury.EntityEntry()
                {
                    Component = fact.Id,
                    Date = fact.OccurredOnDate.HasValue ? fact.OccurredOnDate.Value : fact.DiscoveredOnDate.Value ,
                    IncidentInjuries = fact.IncidentInjuries,
                    FloorMapRoom = floorMapRoom
                });
            }
        }


    }
}
