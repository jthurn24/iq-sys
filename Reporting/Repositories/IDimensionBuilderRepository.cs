using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Models.Dimensions;


namespace IQI.Intuition.Reporting.Repositories
{
    public interface IDimensionBuilderRepository 
    {
        Account GetOrCreateAccount(Guid guid, string name);
       
        Floor GetOrCreateFloor(Guid guid);
       
        Facility GetOrCreateFacility(Guid guid);

        Day GetOrCreateDay(DateTime date);


        Month GetOrCreateMonth(int month, int year);


        InfectionType GetOrCreateInfectionType(string name, string color, string shortName);

        InfectionSite GetOrCreateInfectionSite(string name, InfectionType type);
        
        InfectionClassification GetOrCreateInfectionClassification(string enumName);

        Room GetOrCreateRoom(Guid guid);

        Wing GetOrCreateWing(Guid guid);

        FloorMap GetOrCreateFloorMap(Wing wing);
        FloorMap GetOrCreateFloorMap(Facility facility);

        FloorMapRoom GetOrCreateFloorMapRoom(Room room, FloorMap map);

        AverageType GetOrCreateAverageType(string name);

        FacilityAverageType GetOrCreateAverageTypeForFacility(AverageType averageType, Facility facility);

        IncidentInjury GetOrCreateIncidentInjury(string name);
        IncidentInjuryLevel GetOrCreateIncidentInjuryLevel(string name);
        IncidentLocation GetOrCreateIncidentLocation(string name);
        IncidentType GetOrCreateIncidentType(string name, IncidentTypeGroup group);
        IncidentTypeGroup GetOrCreateIncidentTypeGroup(string name);

        PsychotropicDrugName GetOrCreatePsychotropicDrugName(string name);
        PsychotropicDrugType GetOrCreatePsychotropicDrugType(string name);

        WoundClassification GetOrCreateWoundClassification(string name);
        WoundStage GetOrCreateWoundStage(string name, int? rating);
        WoundSite GetOrCreateWoundSite(string name, int tlX, int tlY, int brX, int brY);
        WoundType GetOrCreateWoundType(string name);


        CatheterType GetOrCreateCatheterType(string name);

        ComplaintType GetOrCreateComplaintType(string name);

        void DeleteFacilityAverageType(FacilityAverageType src);
        void AddFacilityAverageType(FacilityAverageType src);

    }
}
