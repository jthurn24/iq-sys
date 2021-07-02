using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Cubes;
using Virtual = IQI.Intuition.Reporting.Models.Cubes.Virtual;

namespace IQI.Intuition.Reporting.Repositories
{
    public interface ICubeRepository
    {

        IEnumerable<FloorMapRoomInfectionType.EntityEntry> GetFloorMapRoomInfectionTypeByDateRange(Guid floorMap, DateTime startDate, DateTime endDate);
        IEnumerable<FloorMapRoomIncidentType.EntityEntry> GetFloorMapRoomIncidentTypeByDateRange(Guid floorMap, DateTime startDate, DateTime endDate);
        IEnumerable<FloorMapRoomWoundStage.EntityEntry> GetFloorMapRoomWoundStageByDateRange(Guid floorMap, DateTime startDate, DateTime endDate);

        IEnumerable<FacilityMonthCensus> GetFacilityMonthCensus(Guid facility);
        IEnumerable<FacilityMonthCensus> GetFacilityMonthCensus(Guid facility, Guid month);
        IEnumerable<FacilityMonthCensus> GetFacilityMonthCensusByQuarter(Guid facility, Guid quarter);
        IEnumerable<FacilityMonthCensus> GetFacilityMonthCensus(IEnumerable<Guid> facilities, Guid month);
        IEnumerable<FacilityMonthCensus> GetFacilityMonthCensusByQuarter(IEnumerable<Guid> facilities, Guid quarter);

        IEnumerable<FacilityMonthInfectionType.Entry> GetFacilityMonthInfectionType(Guid facility, Guid month);
        IEnumerable<FacilityMonthInfectionType.Entry> GetFacilityMonthInfectionTypeByQuarter(Guid facility, Guid quarter);
        IEnumerable<FacilityMonthInfectionType.Entry> GetFacilityMonthInfectionType(IEnumerable<Guid> facilities, Guid month);
        IEnumerable<FacilityMonthInfectionType.Entry> GetFacilityMonthInfectionTypeByQuarter(IEnumerable<Guid> facilities, Guid quarter);

        IEnumerable<FacilityMonthInfectionType> GetRootFacilityMonthInfectionTypeByQuarter(IEnumerable<Guid> facilities, Guid quarter);

        IEnumerable<FacilityMonthSCAUTI.Entry> GetFacilityMonthSCAUTIByQuarter(Guid facility, Guid quarter);

        IEnumerable<FacilityMonthInfectionSite.Entry> GetFacilityMonthInfectionSiteByQuarter(Guid facility, Guid quarter, Guid? type);

        IEnumerable<FloorMonthInfectionType.Entry> GetFloorMonthInfectionType(Guid facility, Guid month);
        IEnumerable<FloorMonthInfectionType.Entry> GetFloorMonthInfectionTypeByQuarter(Guid facility, Guid quarter);


        IEnumerable<FloorMonthIncidentTypeGroup.Entry> GetFloorMonthIncidentTypeGroup(Guid facility, Guid month);
        IEnumerable<FloorMonthIncidentTypeGroup.Entry> GetFloorMonthIncidentTypeGroupByQuarter(Guid facility, Guid quarter);

        IEnumerable<WingMonthInfectionSite.InfectionSiteEntry> GetWingMonthInfectionType(Guid facility, Guid month);
        IEnumerable<WingMonthInfectionSite.InfectionSiteEntry> GetWingMonthInfectionTypeByQuarter(Guid facility, Guid quarter);

        IEnumerable<WingMonthIncidentTypeGroup.Entry> GetWingMonthIncidentTypeGroup(Guid facility, Guid month);
        IEnumerable<WingMonthIncidentTypeGroup.Entry> GetWingMonthIncidentTypeGroupByQuarter(Guid facility, Guid quarter);

        IEnumerable<AverageTypeMonthInfectionType> GetAverageTypeMonthInfectionType(Guid averageType, Guid month);
        IEnumerable<AverageTypeMonthInfectionType> GetAverageTypeMonthInfectionTypeByQuarter(Guid averageType, Guid quarter);

        IEnumerable<FacilityMonthIncidentType.Entry> GetFacilityMonthIncidentType(Guid facility, Guid month);
        IEnumerable<FacilityMonthIncidentType.Entry> GetFacilityMonthIncidentTypeByQuarter(Guid facility, Guid quarter);
        IEnumerable<FacilityMonthIncidentTypeGroup.Entry> GetFacilityMonthIncidentTypeGroup(Guid facility, Guid month);
        IEnumerable<FacilityMonthIncidentTypeGroup.Entry> GetFacilityMonthIncidentTypeGroupByQuarter(Guid facility, Guid quarter);
        
        IEnumerable<FacilityMonthIncidentInjury.Entry> GetFacilityMonthIncidentInjuryByQuarter(Guid facility, Guid quarter);
        IEnumerable<FacilityMonthIncidentInjury.Entry> GetFacilityMonthIncidentInjuryByRange(Guid facility, int startYear, int endYear, int startMonth, int endMonth);

        IEnumerable<FacilityMonthIncidentInjuryLevel.Entry> GetFacilityMonthIncidentInjuryLevelByQuarter(Guid facility, Guid quarter);
        IEnumerable<FacilityMonthIncidentLocation.Entry> GetFacilityMonthIncidentLocationByQuarter(Guid facility, Guid quarter);
        IEnumerable<FacilityMonthIncidentDayOfWeek.Entry> GetFacilityMonthIncidentDayOfWeekByQuarter(Guid facility, Guid quarter);
        IEnumerable<FacilityMonthIncidentHourOfDay.Entry> GetFacilityMonthIncidentHourOfDayByQuarter(Guid facility, Guid quarter);

        IEnumerable<FacilityInfectionSite.Entry> GetFacilityInfectionSiteForFacility(Guid facility);

        IEnumerable<FacilityMonthPsychotropicDrugType.Entry> GetFacilityMonthPsychotropicDrugTypeByQuarter(Guid facility, Guid quarter);

        IEnumerable<FacilityMonthWoundClassification.Entry> GetFacilityMonthWoundClassificationByQuarter(Guid facility, Guid quarter, Guid woundType);
        IEnumerable<FacilityMonthWoundSite.Entry> GetFacilityMonthWoundSiteByQuarter(Guid facility, Guid quarter, Guid woundType);

        IEnumerable<FacilityMonthWoundStage.Entry> GetFacilityMonthWoundStage(Guid facility, Guid month, Guid woundType);
        IEnumerable<FacilityMonthWoundStage.Entry> GetFacilityMonthWoundStageByQuarter(Guid facility, Guid quarter, Guid woundType);
        IEnumerable<FacilityMonthWoundStage.Entry> GetFacilityMonthWoundStageByRange(Guid facility, int startYear, int endYear, int startMonth, int endMonth);

        IEnumerable<FacilityMonthComplaintType.Entry> GetFacilityMonthComplaintTypeByQuarter(Guid facility, Guid quarter);
        IEnumerable<WingMonthComplaintType.Entry> GetWingMonthComplaintTypeByQuarter(Guid facility, Guid quarter, Guid typeId);
        IEnumerable<FloorMonthComplaintType.Entry> GetFloorMonthComplaintTypeByQuarter(Guid facility, Guid quarter, Guid typeId);

        IEnumerable<FacilityMonthCatheter.Entry> GetFacilityMonthCatheterByQuarter(Guid facility, Guid quarter);

        IEnumerable<WingMonthWoundType.Entry> GetWingMonthWoundTypeByQuarter(Guid facility, Guid quarter, Guid typeId);
    }
}
