using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Utilities;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using System.Linq;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Reporting
{
    public class CubeRepository : ReportingRepository, ICubeRepository
    {
        public CubeRepository(IDocumentStore store)
            : base(store)
        { }



        public IEnumerable<FloorMapRoomInfectionType.EntityEntry> GetFloorMapRoomInfectionTypeByDateRange(Guid floorMap, DateTime startDate, DateTime endDate)
        {

            var data = GetQueryable<FloorMapRoomInfectionType>()
                .Where(src => src.FloorMap.Id == floorMap)
                .FirstOrDefault();

            return data.RoomEntries.SelectMany(x => x.EntityEntries)
                .Where(x => x.StartDate.Date <= endDate &&
                    (x.EndDate == null || startDate <= x.EndDate.Value.Date));
        }

        public IEnumerable<FloorMapRoomIncidentType.EntityEntry> GetFloorMapRoomIncidentTypeByDateRange(Guid floorMap, DateTime startDate, DateTime endDate)
        {

            var data = GetQueryable<FloorMapRoomIncidentType>()
                .Where(src => src.FloorMap.Id == floorMap)
                .FirstOrDefault();

            return data.RoomEntries.SelectMany(x => x.EntityEntries)
                .Where(x => x.Date >= startDate && x.Date <= endDate);
        }

        public IEnumerable<FloorMapRoomWoundStage.EntityEntry> GetFloorMapRoomWoundStageByDateRange(Guid floorMap, DateTime startDate, DateTime endDate)
        {
            var data = GetQueryable<FloorMapRoomWoundStage>()
                .Where(src => src.FloorMap.Id == floorMap)
                .FirstOrDefault();

            return data.RoomEntries.SelectMany(x => x.EntityEntries)
                .Where(x => x.StartDate <= endDate &&
                    (x.EndDate == null || startDate <= x.EndDate));
        }

        public IEnumerable<FacilityMonthCensus> GetFacilityMonthCensus(Guid facility)
        {
            return GetQueryable<FacilityMonthCensus>()
                .Where(src => src.Facility.Id == facility);
        }

        public IEnumerable<FacilityMonthCensus> GetFacilityMonthCensus(Guid facility, Guid month)
        {
            return GetQueryable<FacilityMonthCensus>()
                .Where(src => src.Facility.Id == facility &&
                    src.Month.Id == month);
        }

        public IEnumerable<FacilityMonthCensus> GetFacilityMonthCensusByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthCensus>()
                 .Where(src => src.Facility.Id == facility &&
                     src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthCensus> GetFacilityMonthCensus(IEnumerable<Guid> facilities, Guid month)
        {
            return GetQueryable<FacilityMonthCensus>()
                .Where(src => facilities.Contains(src.Facility.Id) &&
                    src.Month.Id == month);
        }

        public IEnumerable<FacilityMonthCensus> GetFacilityMonthCensusByQuarter(IEnumerable<Guid> facilities, Guid quarter)
        {
            return GetQueryable<FacilityMonthCensus>()
                 .Where(src => facilities.Contains(src.Facility.Id) &&
                     src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthInfectionType.Entry> GetFacilityMonthInfectionType(Guid facility, Guid month)
        {
            return GetQueryable<FacilityMonthInfectionType>()
            .Where(src => src.Facility.Id == facility)
            .First()
            .Entries
            .Where(src => src.Month.Id == month);
        }

        public IEnumerable<FacilityMonthInfectionType.Entry> GetFacilityMonthInfectionType(IEnumerable<Guid> facilities, Guid month)
        {
            return GetQueryable<FacilityMonthInfectionType>()
            .Where(src => facilities.Contains(src.Facility.Id))
            .ToList()
            .SelectMany( x => x.Entries)
            .Where(src => src.Month.Id == month);
        }

        public IEnumerable<FacilityMonthInfectionSite.Entry> GetFacilityMonthInfectionSiteByQuarter(Guid facility, Guid quarter, Guid? type)
        {
            var q = GetQueryable<FacilityMonthInfectionSite>().Where(src => src.Facility.Id == facility)
                .First()
                .Entries
                .Where(src => src.Month.Quarter.Id == quarter);

            if(type.HasValue)
            {
                q = q.Where(x => x.InfectionType.Id == type.Value);
            }

            return q;
        }

        public IEnumerable<FacilityMonthInfectionType.Entry> GetFacilityMonthInfectionTypeByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthInfectionType>()
            .Where(src => src.Facility.Id == facility)
            .First()
            .Entries
            .Where(src => src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthInfectionType.Entry> GetFacilityMonthInfectionTypeByQuarter(IEnumerable<Guid> facilities, Guid quarter)
        {
            return GetQueryable<FacilityMonthInfectionType>()
            .Where(src => facilities.Contains(src.Facility.Id))
            .ToList()
            .SelectMany(x => x.Entries )
            .Where(src => src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthInfectionType> GetRootFacilityMonthInfectionTypeByQuarter(IEnumerable<Guid> facilities, Guid quarter)
        {
            return GetQueryable<FacilityMonthInfectionType>().Where(src => facilities.Contains(src.Facility.Id));
        }

        public IEnumerable<FloorMonthInfectionType.Entry> GetFloorMonthInfectionType(Guid facility, Guid month)
        {
            return GetQueryable<FloorMonthInfectionType>()
            .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                src.Month.Id == month);
        }

        public IEnumerable<FloorMonthInfectionType.Entry> GetFloorMonthInfectionTypeByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FloorMonthInfectionType>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<WingMonthInfectionSite.InfectionSiteEntry> GetWingMonthInfectionType(Guid facility, Guid month)
        {
            var result = GetQueryable<WingMonthInfectionSite>()
            .Where(src => src.Facility.Id == facility)
            .First();

            foreach(var site in result.InfectionSiteEntries)
            {
                foreach(var wing in site.WingEntries)
                {
                    wing.Entries = wing.Entries.Where(x => x.Month.Id == month).ToList();
                }
            }

            return result.InfectionSiteEntries;
        }

        public IEnumerable<WingMonthInfectionSite.InfectionSiteEntry> GetWingMonthInfectionTypeByQuarter(Guid facility, Guid quarter)
        {
            var result = GetQueryable<WingMonthInfectionSite>()
            .Where(src => src.Facility.Id == facility)
            .First();

            foreach (var site in result.InfectionSiteEntries)
            {
                foreach (var wing in site.WingEntries)
                {
                    wing.Entries = wing.Entries.Where(x => x.Month.Quarter.Id == quarter).ToList();
                }
            }

            return result.InfectionSiteEntries;
        }

        public IEnumerable<AverageTypeMonthInfectionType> GetAverageTypeMonthInfectionType(Guid averageType, Guid month)
        {
            return GetQueryable<AverageTypeMonthInfectionType>()
            .Where(src => src.AverageType.Id == averageType &&
                src.Month.Id == month);
        }

        public IEnumerable<AverageTypeMonthInfectionType> GetAverageTypeMonthInfectionTypeByQuarter(Guid averageType, Guid quarter)
        {
            return GetQueryable<AverageTypeMonthInfectionType>()
             .Where(src => src.AverageType.Id == averageType &&
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthIncidentType.Entry> GetFacilityMonthIncidentType(Guid facility, Guid month)
        {
            return GetQueryable<FacilityMonthIncidentType>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Id == month);
        }

        public IEnumerable<FacilityMonthIncidentType.Entry> GetFacilityMonthIncidentTypeByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthIncidentType>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthIncidentInjury.Entry> GetFacilityMonthIncidentInjuryByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthIncidentInjury>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthIncidentInjury.Entry> GetFacilityMonthIncidentInjuryByRange(Guid facility, int startYear, int endYear, int startMonth, int endMonth)
        {
            return GetQueryable<FacilityMonthIncidentInjury>()
             .Where(src => src.Facility.Id == facility).First().Entries
             .Where(src => src.Month.Year > startYear || (src.Month.Year == startYear && src.Month.MonthOfYear >= startMonth))
             .Where(src => src.Month.Year < endYear || (src.Month.Year == endYear && src.Month.MonthOfYear <= endMonth));

        }

        public IEnumerable<FacilityMonthIncidentInjuryLevel.Entry> GetFacilityMonthIncidentInjuryLevelByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthIncidentInjuryLevel>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthIncidentLocation.Entry> GetFacilityMonthIncidentLocationByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthIncidentLocation>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthIncidentDayOfWeek.Entry> GetFacilityMonthIncidentDayOfWeekByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthIncidentDayOfWeek>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter)
                 .OrderBy(x => x.DayOfWeek);
        }

        public IEnumerable<FacilityMonthIncidentHourOfDay.Entry> GetFacilityMonthIncidentHourOfDayByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthIncidentHourOfDay>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter)
                 .OrderBy(x => x.HourOfDay);
        }

        public IEnumerable<FacilityMonthIncidentTypeGroup.Entry> GetFacilityMonthIncidentTypeGroup(Guid facility, Guid month)
        {
            return GetQueryable<FacilityMonthIncidentTypeGroup>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Id == month);
        }

        public IEnumerable<FloorMonthIncidentTypeGroup.Entry> GetFloorMonthIncidentTypeGroup(Guid facility, Guid month)
        {
            return GetQueryable<FloorMonthIncidentTypeGroup>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Id == month);
        }

        public IEnumerable<FloorMonthIncidentTypeGroup.Entry> GetFloorMonthIncidentTypeGroupByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FloorMonthIncidentTypeGroup>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<WingMonthIncidentTypeGroup.Entry> GetWingMonthIncidentTypeGroup(Guid facility, Guid month)
        {
            return GetQueryable<WingMonthIncidentTypeGroup>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Id == month);
        }

        public IEnumerable<WingMonthIncidentTypeGroup.Entry> GetWingMonthIncidentTypeGroupByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<WingMonthIncidentTypeGroup>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthIncidentTypeGroup.Entry> GetFacilityMonthIncidentTypeGroupByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthIncidentTypeGroup>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityInfectionSite.Entry> GetFacilityInfectionSiteForFacility(Guid facility)
        {
            return GetQueryable<FacilityInfectionSite>()
             .Where(src => src.Facility.Id == facility)
             .First()
             .Entries;
        }

        public IEnumerable<FacilityMonthPsychotropicDrugType.Entry> GetFacilityMonthPsychotropicDrugTypeByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthPsychotropicDrugType>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthWoundClassification.Entry> GetFacilityMonthWoundClassificationByQuarter(Guid facility, Guid quarter, Guid typeId)
        {
            return GetQueryable<FacilityMonthWoundClassification>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter &&
                 src.WoundType.Id == typeId);
        }

        public IEnumerable<FacilityMonthWoundSite.Entry> GetFacilityMonthWoundSiteByQuarter(Guid facility, Guid quarter, Guid typeId)
        {
            return GetQueryable<FacilityMonthWoundSite>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter &&
                 src.WoundType.Id  == typeId);
        }

        public IEnumerable<FacilityMonthWoundStage.Entry> GetFacilityMonthWoundStage(Guid facility, Guid month, Guid typeId)
        {
            return GetQueryable<FacilityMonthWoundStage>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Id == month &&
                 src.WoundType.Id == typeId);
        }

        public IEnumerable<FacilityMonthWoundStage.Entry> GetFacilityMonthWoundStageByQuarter(Guid facility, Guid quarter, Guid typeId)
        {
            return GetQueryable<FacilityMonthWoundStage>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter &&
                 src.WoundType.Id == typeId);
        }

        public IEnumerable<FacilityMonthWoundStage.Entry> GetFacilityMonthWoundStageByRange(Guid facility, int startYear, int endYear, int startMonth, int endMonth)
        {
            return GetQueryable<FacilityMonthWoundStage>()
             .Where(src => src.Facility.Id == facility).First().Entries
             .Where(src => src.Month.Year > startYear || (src.Month.Year == startYear && src.Month.MonthOfYear >= startMonth))
             .Where(src => src.Month.Year < endYear || (src.Month.Year == endYear && src.Month.MonthOfYear <= endMonth));

        }

        public IEnumerable<FacilityMonthComplaintType.Entry> GetFacilityMonthComplaintTypeByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthComplaintType>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<WingMonthComplaintType.Entry> GetWingMonthComplaintTypeByQuarter(Guid facility, Guid quarter, Guid typeId)
        {
            return GetQueryable<WingMonthComplaintType>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.ComplaintType.Id == typeId &&
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FloorMonthComplaintType.Entry> GetFloorMonthComplaintTypeByQuarter(Guid facility, Guid quarter, Guid typeId)
        {
            return GetQueryable<FloorMonthComplaintType>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.ComplaintType.Id == typeId &&
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthCatheter.Entry> GetFacilityMonthCatheterByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthCatheter>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<WingMonthWoundType.Entry> GetWingMonthWoundTypeByQuarter(Guid facility, Guid quarter, Guid typeId)
        {
            return GetQueryable<WingMonthWoundType>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.WoundType.Id == typeId &&
                 src.Month.Quarter.Id == quarter);
        }

        public IEnumerable<FacilityMonthSCAUTI.Entry> GetFacilityMonthSCAUTIByQuarter(Guid facility, Guid quarter)
        {
            return GetQueryable<FacilityMonthSCAUTI>()
             .Where(src => src.Facility.Id == facility).First().Entries.Where(src =>
                 src.Month.Quarter.Id == quarter);
        }

    }
}
