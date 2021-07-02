using System;
using System.Collections.Generic;
using System.Linq;
using RedArrow.Framework.Extensions.Common;
using RedArrow.Framework.Persistence;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Reporting.Containers;
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Reporting
{
    public class DimensionRepository : ReportingRepository, IDimensionRepository
    {
        public DimensionRepository(IDocumentStore store)
            : base(store)
             { }


        public Quarter GetQuarter(Guid id)
        {
            return GetQueryable<Quarter>()
           .Where(q => q.Id == id)
           .FirstOrDefault();
        }

        public QuarterMonths GetQuarterMonths(Guid id)
        {
            var q = GetQueryable<Quarter>()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (q == null)
            {
                return null;
            }

            var r = new QuarterMonths();
            r.Quarter = q;

            var months = GetQueryable<Month>().Where(x => x.Quarter.Id == id)
                .ToList()
                .OrderBy(x => x.MonthOfYear);


            r.Month1 = months.ToArray()[0];
            r.Month2 = months.ToArray()[1];
            r.Month3 = months.ToArray()[2];

            return r;
        }

        public IEnumerable<Floor> GetFloors(Facility facility)
        {
            return GetQueryable<Floor>()
            .Where(floor => floor.Facility.Id == facility.Id)
            ;
        }

        public IEnumerable<InfectionType> GetInfectionTypes()
        {
            return GetQueryable<InfectionType>().OrderBy(x => x.SortOrder)
            ;
        }

        public IEnumerable<FloorMap> GetFloorMapsForAccount(Guid id)
        {
            return GetQueryable<FloorMap>()
            .Where(src => src.Facility.Account.Id == id)
            ;
        }

        public IEnumerable<FloorMap> GetFloorMapsForFacility(Guid id)
        {
            return GetQueryable<FloorMap>()
            .Where(m => m.Facility.Id == id)
            ;
        }

        public IEnumerable<FloorMapRoom> GetFloorMapRoomsForFloorMap(Guid id)
        {
            return GetQueryable<FloorMapRoom>()
            .Where(m => m.FloorMap.Id == id)
            ;
        }

        public FloorMapRoom GetFloorMapRoom(Guid id)
        {
            return GetQueryable<FloorMapRoom>()
            .Where(m => m.Id == id)
            .FirstOrDefault();
        }

        public Facility GetFacility(Guid id)
        {
            return GetQueryable<Facility>()
                .Where(src => src.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<InfectionClassification> GetInfectionClassifications()
        {
            return GetQueryable<InfectionClassification>()
                ;
        }

        public IEnumerable<Month> GetAllMonths()
        {
            return GetQueryable<Month>()
                ;
        }

        public IEnumerable<Facility> GetAllFacilities()
        {
            return GetQueryable<Facility>()
                ;
        }

        public IEnumerable<Day> GetAllDays()
        {
            return GetQueryable<Day>()
                ;
        }

        public IEnumerable<Quarter> GetNonFutureQuarters()
        {
            var allQuarters = GetAllQuarters();

            int quarter;

            if (DateTime.Today.Month < 4)
            {
                quarter = 1;
            }
            else if(DateTime.Today.Month < 7)
            {
                quarter = 2;
            }
            else if (DateTime.Today.Month < 10)
            {
                quarter = 3;
            }
            else
            {
                quarter = 4;
            }

            return allQuarters
                .Where(x => x.Year < DateTime.Today.Year
                    || (x.Year == DateTime.Today.Year && x.QuarterOfYear <= quarter));
        }

        public IEnumerable<Quarter> GetAllQuarters()
        {
            return GetQueryable<Quarter>()
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.QuarterOfYear);
        }

        public FloorMap GetFloorMap(Guid id)
        {
            return GetQueryable<FloorMap>()
                .Where(src => src.Id == id)
                .FirstOrDefault();
        }

        public FloorMapImage GetFloorMapImage(Guid id)
        {
            return GetQueryable<FloorMapImage>()
                .Where(src => src.FloorMap.Id == id)
                .FirstOrDefault();
        }


        public IEnumerable<FacilityAverageType> GetFacilityAverageTypesForFacility(Guid facility)
        {
            return GetQueryable<FacilityAverageType>()
            .Where(src => src.Facility.Id == facility);
        }

        public IEnumerable<FacilityAverageType> GetFacilityAverageTypesForAccount(Guid account)
        {
            return GetQueryable<FacilityAverageType>()
            .Where(src => src.Facility.Account.Id == account)
            .Distinct(x => x.AverageType.Id);
        }

        public AverageType GetAverageType(Guid id)
        {
            return GetQueryable<AverageType>()
            .Where(src => src.Id == id)
            .FirstOrDefault();
        }

        public IEnumerable<IncidentInjury> GetIncidentInjuries()
        {
            return GetQueryable<IncidentInjury>().OrderBy(x => x.SortOrder);
        }

        public IEnumerable<IncidentInjuryLevel> GetIncidentInjuryLevels()
        {
            return GetQueryable<IncidentInjuryLevel>();
        }

        public IEnumerable<IncidentLocation> GetIncidentLocations()
        {
            return GetQueryable<IncidentLocation>();
        }

        public IEnumerable<IncidentType> GetIncidentTypes()
        {
            return GetQueryable<IncidentType>().OrderBy(x => x.SortOrder);
        }

        public IEnumerable<IncidentTypeGroup> GetIncidentTypeGroups()
        {
            return GetQueryable<IncidentTypeGroup>();
        }

        public Account GetAccount(Guid id)
        {
            return GetQueryable<Account>()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Facility> GetFacilities(IEnumerable<Guid> guids)
        {
            var g = guids.ToArray();
            return GetQueryable<Facility>()
                .Where(x => g.Contains(x.Id));
        }

        public Month GetMonth(int year, int month)
        {
            return GetQueryable<Month>()
                .Where(x => x.Year == year && x.MonthOfYear == month)
                
                .FirstOrDefault();
        }

        public Month GetMonth(DateTime date)
        {
            return GetMonth(date.Year, date.Month);
        }

        public IEnumerable<PsychotropicDrugType> GetAllPsychotropicDrugTypes()
        {
            return GetQueryable<PsychotropicDrugType>();
        }


        public IEnumerable<InfectionSite> GetInfectionSites()
        {
            return GetQueryable<InfectionSite>();
        }

        public IEnumerable<WoundStage> GetWoundStages()
        {
            return GetQueryable<WoundStage>();
        }

        public IEnumerable<WoundType> GetWoundTypes()
        {
            return GetQueryable<WoundType>();
        }



        public IEnumerable<WoundClassification> GetWoundClassifications()
        {
            return GetQueryable<WoundClassification>() ;
        }

        public IEnumerable<WoundSite> GetWoundSites()
        {
            return GetQueryable<WoundSite>();
        }

        public IEnumerable<ComplaintType> GetComplaintTypes()
        {
            return GetQueryable<ComplaintType>();
        }

        public IEnumerable<CatheterType> GetCatheterTypes()
        {
            return GetQueryable<CatheterType>();
        }

        public IEnumerable<Wing> GetWingsForFacility(Guid id)
        {
            return GetQueryable<Wing>()
                .Where(x => x.Floor.Facility.Id == id);
        }

        public IEnumerable<Floor> GetFloorsForFacility(Guid id)
        {
            return GetQueryable<Floor>()
                .Where(x => x.Facility.Id == id);
        }

        public IEnumerable<FacilityAverageType> GetFacilityAverageTypesForType(Guid id)
        {
            return GetQueryable<FacilityAverageType>()
                .ToList()
                .Where(x => x.AverageType.Id == id);
        }

        public IEnumerable<Facility> GetFacilities()
        {
            return GetQueryable<Facility>().ToList();
        }

        public IEnumerable<AverageType> GetAllAverageTypes()
        {
            return GetQueryable<AverageType>().ToList();
        }
    }
}
