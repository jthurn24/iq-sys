using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Reporting.Repositories
{
    public interface IDimensionRepository 
    {


        FloorMap GetFloorMap(Guid id);
        FloorMapImage GetFloorMapImage(Guid id);

        IEnumerable<FloorMap> GetFloorMapsForAccount(Guid id);
        IEnumerable<FloorMap> GetFloorMapsForFacility(Guid id);
        IEnumerable<FloorMapRoom> GetFloorMapRoomsForFloorMap(Guid id);
        FloorMapRoom GetFloorMapRoom(Guid id);

        Facility GetFacility(Guid id);
        Account GetAccount(Guid id);

        IEnumerable<Facility> GetFacilities(IEnumerable<Guid> guids);
        IEnumerable<Facility> GetFacilities();

        //IEnumerable<Day> GetDayRange(DateTime startDate, DateTime enddate);

        IEnumerable<Floor> GetFloors(Facility facility);

        IEnumerable<InfectionType> GetInfectionTypes();
        IEnumerable<InfectionSite> GetInfectionSites();
        IEnumerable<InfectionClassification> GetInfectionClassifications();

        IEnumerable<IncidentInjury> GetIncidentInjuries();
        IEnumerable<IncidentInjuryLevel> GetIncidentInjuryLevels();
        IEnumerable<IncidentLocation> GetIncidentLocations();
        IEnumerable<IncidentType> GetIncidentTypes();
        IEnumerable<IncidentTypeGroup> GetIncidentTypeGroups();


        Quarter GetQuarter(Guid id);
        Month GetMonth(int year, int month);
        Month GetMonth(DateTime date);
        QuarterMonths GetQuarterMonths(Guid id);

        IEnumerable<Quarter> GetAllQuarters();
        IEnumerable<Quarter> GetNonFutureQuarters();
        IEnumerable<Month> GetAllMonths();
        IEnumerable<Day> GetAllDays();
        IEnumerable<Facility> GetAllFacilities();

        IEnumerable<FacilityAverageType> GetFacilityAverageTypesForFacility(Guid facility);
        IEnumerable<FacilityAverageType> GetFacilityAverageTypesForAccount(Guid account);
        IEnumerable<FacilityAverageType> GetFacilityAverageTypesForType(Guid id);


        IEnumerable<AverageType> GetAllAverageTypes();
        AverageType GetAverageType(Guid id);

        IEnumerable<PsychotropicDrugType> GetAllPsychotropicDrugTypes();

        IEnumerable<WoundStage> GetWoundStages();
        IEnumerable<WoundType> GetWoundTypes();
        IEnumerable<WoundClassification> GetWoundClassifications();
        IEnumerable<WoundSite> GetWoundSites();

        IEnumerable<ComplaintType> GetComplaintTypes();

        IEnumerable<Wing> GetWingsForFacility(Guid id);
        IEnumerable<Floor> GetFloorsForFacility(Guid id);

        IEnumerable<CatheterType> GetCatheterTypes();
    }
}
