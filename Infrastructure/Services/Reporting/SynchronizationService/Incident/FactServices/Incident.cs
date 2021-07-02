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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Incident.FactServices
{
    public class Incident : AbstractFactService
    {


        public Incident(
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
            IList<Domain.Models.IncidentReport> dIncidents,
            Domain.Models.Facility dFacility,
            DataDimensions dimensions)
        {

            foreach (var dIncident in dIncidents)
            {
                if (dIncident.DiscoveredOn.HasValue)
                {
                    _Log.Info(string.Format("Syncing incident: {0}  facility: {1}", dIncident.Guid, dFacility.Name));



                    var dPatient = _DataContext.Fetch<Patient>(dIncident.Patient.Id);
                    var dRoom = _DataContext.Fetch<Room>(dIncident.Room.Id);
                    var dWing = _DataContext.Fetch<Wing>(dRoom.Wing.Id);
                    var dFloor = _DataContext.Fetch<Floor>(dWing.Floor.Id);
                    var dAccount = _DataContext.Fetch<Domain.Models.Account>(dFacility.Account.Id);

                    var dIncidentTypes = _DataContext.CreateQuery<Domain.Models.IncidentType>()
                        .FilterBy(x => x.IncidentReports.Contains(dIncident)).FetchAll();

                    var dIncidentLocation = _DataContext.Fetch<Domain.Models.IncidentLocation>(dIncident.IncidentLocation.Id);

                    var dIncidentInjuries = _DataContext.CreateQuery<Domain.Models.IncidentInjury>()
                        .FilterBy(x => x.IncidentReports.Contains(dIncident)).FetchAll();


                    var room = _DimensionBuilderRepository.GetOrCreateRoom(dRoom.Guid);
                    var wing = _DimensionBuilderRepository.GetOrCreateWing(dWing.Guid);
                    var floor = _DimensionBuilderRepository.GetOrCreateFloor(dFloor.Guid);
                    var account = _DimensionBuilderRepository.GetOrCreateAccount(dAccount.Guid,dAccount.Name);
                    var facility = _DimensionBuilderRepository.GetOrCreateFacility(dFacility.Guid);

                    Dimensions.FloorMap floorMap;

                    if (facility.HasSingleFloorMap.HasValue && facility.HasSingleFloorMap.Value == true)
                    {
                        floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(facility);
                    }
                    else
                    {
                        floorMap = _DimensionBuilderRepository.GetOrCreateFloorMap(wing);
                    }


                    var floorMapRoom = _DimensionBuilderRepository.GetOrCreateFloorMapRoom(room, floorMap);


                    var incidentLocation = _DimensionBuilderRepository.GetOrCreateIncidentLocation(dIncidentLocation.Name);
                    incidentLocation.Color = dIncidentLocation.Color;
                    Save<Dimensions.IncidentLocation>(incidentLocation);

                    var incidentInjuryLevel = _DimensionBuilderRepository.GetOrCreateIncidentInjuryLevel(System.Enum.GetName(typeof(Domain.Enumerations.InjuryLevel), dIncident.InjuryLevel));
                    incidentInjuryLevel.Color = IQI.Intuition.Reporting.Graphics.Helpers.ColorScheme.GetDefaultHtmlColor((int)dIncident.InjuryLevel);
                    Save<Dimensions.IncidentInjuryLevel>(incidentInjuryLevel);

                    var record = _FactBuilderRespository.GetOrCreateIncidentReport(dIncident.Guid);

                    
                    TrackDimensionChanges(dimensions, 
                        record);

                    record.Room = room;
                    record.Wing = wing;
                    record.Floor = floor;
                    record.Account = account;
                    record.Facility = facility;
                    record.FloorMap = floorMap;
                    record.FloorMapRoom = floorMapRoom;
                    record.Deleted = dIncident.Deleted;


                    if (dIncident.OccurredOn.HasValue)
                    {
                        record.OccurredOnDate = dIncident.OccurredOn;
                        record.Day = _DimensionBuilderRepository.GetOrCreateDay(dIncident.OccurredOn.Value);
                        record.Month = _DimensionBuilderRepository.GetOrCreateMonth(dIncident.OccurredOn.Value.Month, dIncident.OccurredOn.Value.Year);
                        record.Quarter = record.Month.Quarter;
                        record.OccurredOnDayOfWeek = (int)record.OccurredOnDate.Value.DayOfWeek;
                        record.OccurredOnHourOfDay = dIncident.OccurredOn.Value.Hour;
                    }
                    else
                    {
                        record.OccurredOnDayOfWeek = null;
                        record.OccurredOnHourOfDay = null;
                        record.OccurredOnDate = null;

                        record.Day = _DimensionBuilderRepository.GetOrCreateDay(dIncident.DiscoveredOn.Value);
                        record.Month = _DimensionBuilderRepository.GetOrCreateMonth(dIncident.DiscoveredOn.Value.Month, dIncident.DiscoveredOn.Value.Year);
                        record.Quarter = record.Month.Quarter;
                    }


                    record.DiscoveredOnDate = dIncident.DiscoveredOn;
                    record.DiscoveredOnDayOfWeek = (int)record.DiscoveredOnDate.Value.DayOfWeek;
                    record.DiscoveredOnHourOfDay = dIncident.DiscoveredOn.Value.Hour;


                    record.IncidentLocation = incidentLocation;
                    record.IncidentInjuryLevel = incidentInjuryLevel;

                  
                    /* refresh sub collections */


                    record.IncidentInjuries = new List<Dimensions.IncidentInjury>();
                    foreach (var d in dIncidentInjuries)
                    {
                        var dd = _DimensionBuilderRepository.GetOrCreateIncidentInjury(d.Name);
                        dd.Color = d.Color;
                        record.IncidentInjuries.Add(dd);
                        Save<Dimensions.IncidentInjury>(dd);
                    }


                    record.IncidentTypes = new List<Dimensions.IncidentType>();
                    record.IncidentTypeGroups = new List<Dimensions.IncidentTypeGroup>();
                    foreach (var d in dIncidentTypes)
                    {
                        var dg = _DimensionBuilderRepository.GetOrCreateIncidentTypeGroup(d.GroupName);
                        dg.Color = d.GroupColor;
                        record.IncidentTypeGroups.Add(dg);
                        Save<Dimensions.IncidentTypeGroup>(dg);


                        var dd = _DimensionBuilderRepository.GetOrCreateIncidentType(d.Name,dg);
                        dd.Color = d.Color;
                        record.IncidentTypes.Add(dd);
                        Save<Dimensions.IncidentType>(dd);
                    }



                    Save<Facts.IncidentReport>(record);
                    
                    /* Track changes */ 

                    TrackDimensionChanges(dimensions,
                        record);

                }
            }

            //System.IO.File.AppendAllText(string.Concat("c:\\",DateTime.Today.ToString("MM-dd-yyyy"),".log"), _TempBuilder.ToString());
        }


        private void TrackDimensionChanges(DataDimensions dimensions, 
            Facts.IncidentReport incident
            )
        {

            foreach (var typeGroup in incident.IncidentTypeGroups)
            {
                if (dimensions.IncidentTypeGroups.Select(x => x.Name).Contains(typeGroup.Name) == false)
                {
                    dimensions.IncidentTypeGroups.Add(typeGroup);
                }
            }

            foreach (var type in incident.IncidentTypes)
            {
                if (dimensions.IncidentTypes.Select(x => x.Name).Contains(type.Name) == false)
                {
                    dimensions.IncidentTypes.Add(type);
                }
            }

            foreach (var injury in incident.IncidentInjuries)
            {
                if (dimensions.IncidentInjuries.Select(x => x.Name).Contains(injury.Name) == false)
                {
                    dimensions.IncidentInjuries.Add(injury);
                }
            }

            if (incident.IncidentInjuryLevel != null)
            {
                if (dimensions.IncidentInjuryLevels.Count(x => x.Name == incident.IncidentInjuryLevel.Name) < 1)
                {
                    dimensions.IncidentInjuryLevels.Add(incident.IncidentInjuryLevel);
                }
            }

            if (incident.IncidentLocation != null)
            {
                if (dimensions.IncidentLocations.Count(x => x.Name == incident.IncidentLocation.Name) < 1)
                {
                    dimensions.IncidentLocations.Add(incident.IncidentLocation);
                }
            }

            if (incident.Floor != null)
            {
                if (dimensions.Floors.Count(x => x.Id == incident.Floor.Id) < 1)
                {
                    dimensions.Floors.Add(incident.Floor);
                }
            }

            if (incident.Wing != null)
            {
                if (dimensions.Wings.Count(x => x.Id == incident.Wing.Id) < 1)
                {
                    dimensions.Wings.Add(incident.Wing);
                }
            }

            TrackDimensionDateChanges(incident.OccurredOnDate, dimensions);
            TrackDimensionDateChanges(incident.DiscoveredOnDate, dimensions);

          
        }

        private void TrackDimensionDateChanges(DateTime? date, DataDimensions dimensions)
        {
            if (date.HasValue)
            {
                DateTime endDate = DateTime.Today;

                if (dimensions.StartDate.HasValue == false || dimensions.StartDate.Value > date.Value)
                {
                    dimensions.StartDate = date.Value.Date;
                }

                if (dimensions.EndDate.HasValue == false || dimensions.EndDate.Value < endDate)
                {
                    dimensions.EndDate = endDate.Date;
                }

            }
        }





    }
}
