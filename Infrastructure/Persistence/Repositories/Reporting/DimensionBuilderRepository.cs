using RedArrow.Framework.Extensions.Common;
using IQI.Intuition.Reporting.Models.Dimensions;
using IQI.Intuition.Reporting.Models.Cubes;
using IQI.Intuition.Reporting.Repositories;
using RedArrow.Framework.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using System.Linq;
using System;
using SnyderIS.sCore.Persistence;

namespace IQI.Intuition.Infrastructure.Persistence.Repositories.Reporting
{
    public class DimensionBuilderRepository : ReportingRepository, IDimensionBuilderRepository
    {

        public DimensionBuilderRepository(IDocumentStore store)
            : base(store)
        { }

        public Account GetOrCreateAccount(Guid guid, string name)
        {

            var result = GetQueryable<Account>()
                .Where(src => src.Id == guid)
                .FirstOrDefault();

            if (result == null)
            {
                result = new Account()
                {
                    Id = guid,
                    Name = name
                };

                _Store.Save<Account>(result);
            }

            return result;
        }

        public Floor GetOrCreateFloor(Guid guid)
        {

            var result = GetQueryable<Floor>()
            .Where(src => src.Id == guid)
            .FirstOrDefault();

            if (result == null)
            {
                result = new Floor()
                {
                    Id = guid
                };

                _Store.Save<Floor>(result);
            }

            return result;
        }

        public Facility GetOrCreateFacility(Guid guid)
        {
            var result = GetQueryable<Facility>()
            .Where(src => src.Id == guid)
            .FirstOrDefault();

            if (result == null)
            {
                result = new Facility()
                {
                    Id = guid
                };

                _Store.Save<Facility>(result);
            }

            return result;
        }

        public Day GetOrCreateDay(DateTime date)
        {
            var cleanDate = new DateTime(date.Year, date.Month, date.Day);

            var result = GetQueryable<Day>()
           .Where(day => day.DayOfMonth == cleanDate.Day &&
               day.MonthOfYear == cleanDate.Month &&
               day.Year == cleanDate.Year)
           .FirstOrDefault();

            if (result == null)
            {
                var month = GetOrCreateMonth(date.Month, date.Year);

                result = new Day()
                {
                    DayOfMonth = date.Day,
                    MonthOfYear = date.Month,
                    Year = date.Year,
                };

                _Store.Save<Day>(result);
            }

            return result;
        }

        public Month GetOrCreateMonth(int month, int year)
        {
            var result = GetQueryable<Month>()
                .Where(src => src.MonthOfYear == month && src.Year == year)
                .FirstOrDefault();

            if (result == null)
            {
                int quarter = 1;

                if(month <= 3)
                {
                    quarter = 1;
                }
                else if(month <=6)
                {
                    quarter = 2;
                }
                else if(month <=9 )
                {
                    quarter = 3;
                }
                else{
                    quarter = 4;
                }

                var q = GetQueryable<Quarter>()
                    .Where(src => src.QuarterOfYear == quarter && src.Year == year)
                    .FirstOrDefault();

                if (q == null)
                {
                    q = new Quarter()
                    {
                        QuarterOfYear = quarter,
                        Year = year
                    };

                    _Store.Save<Quarter>(q);

                    // If we are going to create a quarter, we have to create associated months as well,
                    // otherwise the quarter is not properly represented.

                    for (int i = 1; i <= 3; i++)
                    {
                        if (GetQueryable<Month>().Where(x => x.Year == year && x.MonthOfYear == ((quarter - 1) * 3) + i).Count() < 1)
                        {

                            var m = new Month()
                            {
                                MonthOfYear = ((quarter - 1) * 3) + i,
                                Year = year,
                                Name = new DateTime(year, ((quarter - 1) * 3) + i, 1).ToString("MMMM"),
                                Quarter = q
                            };

                            _Store.Save<Month>(m);

                            if (m.MonthOfYear == month)
                            {
                                result = m;
                            }
                        }
                    }

                }
                else
                {
                    // Since we usually create all months of a quarter at once.. this shouldn't happen.
                    // In the event that it does, we create the month.

                    result = new Month()
                    {
                        MonthOfYear = month,
                        Year = year,
                        Name = new DateTime(year, month, 1).ToString("MMMM"),
                        Quarter = q
                    };

                    _Store.Save<Month>(result);
                }

            }

            return result;
        }

        public InfectionType GetOrCreateInfectionType(string name, string color, string shortName)
        {
            var result = GetQueryable<InfectionType>()
                .Where(src => src.Name == name)
                .FirstOrDefault();

            if (result == null)
            {
                result = new InfectionType()
                {
                    Name = name
                };

                _Store.Save<InfectionType>(result);
            }

            if (result.Color != color)
            {
                result.Color = color;
                _Store.Save<InfectionType>(result);
            }


            if (result.ShortName != shortName)
            {
                result.ShortName = shortName;
                _Store.Save<InfectionType>(result);
            }

            return result;
        }

        public InfectionSite GetOrCreateInfectionSite(string name, InfectionType type)
        {
            var result = GetQueryable<InfectionSite>()
                .Where(src => src.Name == name && src.InfectionType.Id == type.Id)
                .FirstOrDefault();

            if (result == null)
            {
                result = new InfectionSite()
                {
                    Name = name,
                    InfectionType = type
                };

                _Store.Save<InfectionSite>(result);
            }

            return result;
        }

        public InfectionClassification GetOrCreateInfectionClassification(string enumName)
        {
            var result = GetQueryable<InfectionClassification>()
                .Where(src => src.EnumName == enumName)
                .FirstOrDefault();

            if (result == null)
            {
                result = new InfectionClassification()
                {
                    EnumName = enumName
                };

                if (enumName == "HealthCareAssociatedInfection")
                {
                    result.IsNosocomial = true;
                    result.IsQualified = true;
                }

                if (enumName == "Admission")
                {
                    result.IsQualified = true;
                }

                if (enumName == "AdmissionHospitalDiagnosed")
                {
                    result.IsQualified = true;
                }

                _Store.Save<InfectionClassification>(result);
            }

            return result;
        }

        public AverageType GetOrCreateAverageType(string name)
        {
            var result = GetQueryable<AverageType>()
            .Where(src => src.Name == name)
            .FirstOrDefault();

            if (result == null)
            {
                result = new AverageType()
                {
                    Name = name
                };

                _Store.Save<AverageType>(result);
            }

            return result;
        }

        public CatheterType GetOrCreateCatheterType(string name)
        {
            var result = GetQueryable<CatheterType>()
            .Where(src => src.Name == name)
            .FirstOrDefault();

            if (result == null)
            {
                result = new CatheterType()
                {
                    Name = name
                };

                _Store.Save<CatheterType>(result);
            }

            return result;
        }

        public FacilityAverageType GetOrCreateAverageTypeForFacility(AverageType averageType, Facility facility)
        {
            var result = GetQueryable<FacilityAverageType>()
                .Where(src => src.Facility == facility && src.AverageType == averageType)
                .FirstOrDefault();

            if (result == null)
            {
                result = new FacilityAverageType()
                {
                    AverageType = averageType,
                    Facility = facility
                };

                _Store.Save<FacilityAverageType>(result);
            }

            return result;
        }

        public Room GetOrCreateRoom(Guid guid)
        {
            var result = GetQueryable<Room>()
                 .Where(src => src.Id == guid)
                 .FirstOrDefault();

            if (result == null)
            {
                result = new Room()
                {
                    Id = guid
                };

                _Store.Save<Room>(result);
            }

            return result;
        }

        public Wing GetOrCreateWing(Guid guid)
        {
            var result = GetQueryable<Wing>()
            .Where(src => src.Id == guid)
            .FirstOrDefault();

            if (result == null)
            {
                result = new Wing()
                {
                    Id = guid
                };

                _Store.Save<Wing>(result);
            }

            return result;
        }

        public FloorMap GetOrCreateFloorMap(Wing wing)
        {
            var result = GetQueryable<FloorMap>()
                .Where(src => src.Wing.Id == wing.Id)
                .FirstOrDefault();

            if (result == null)
            {
                result = new FloorMap()
                {
                    Facility = wing.Facility,
                    Wing = wing,
                    Active = true,
                    Id = GuidHelper.NewGuid()
                };

                _Store.Save<FloorMap>(result);
            }

            return result;
        }

        public FloorMap GetOrCreateFloorMap(Facility facility)
        {
            var result = GetQueryable<FloorMap>()
                .Where(src => src.Facility.Id == facility.Id && src.Wing == null && src.Active == true)
                .FirstOrDefault();

            if (result == null)
            {
                result = new FloorMap()
                {
                    Facility = facility,
                    Active = true,
                    Id = GuidHelper.NewGuid()
                };

                _Store.Save<FloorMap>(result);
            }

            return result;
        }

        public FloorMapRoom GetOrCreateFloorMapRoom(Room room, FloorMap map)
        {
            var result = GetQueryable<FloorMapRoom>()
              .Where(src => src.Room == room && src.FloorMap == map)
              .FirstOrDefault();

            if (result == null)
            {
                result = new FloorMapRoom()
                {
                    Room = room,
                    FloorMap = map
                };

                _Store.Save<FloorMapRoom>(result);
            }

            return result;
        }

        public IncidentInjury GetOrCreateIncidentInjury(string name)
        {
            var result = GetQueryable<IncidentInjury>()
                .Where(src => src.Name == name)
                .FirstOrDefault();

            if (result == null)
            {
                result = new IncidentInjury()
                {
                    Name = name
                };

                _Store.Save<IncidentInjury>(result);
            }

            return result;
        }

        public IncidentInjuryLevel GetOrCreateIncidentInjuryLevel(string name)
        {
            var result = GetQueryable<IncidentInjuryLevel>()
                .Where(src => src.Name == name)
                .FirstOrDefault();

            if (result == null)
            {
                result = new IncidentInjuryLevel()
                {
                    Name = name
                };

                _Store.Save<IncidentInjuryLevel>(result);
            }

            return result;
        }

        public IncidentLocation GetOrCreateIncidentLocation(string name)
        {
            var result = GetQueryable<IncidentLocation>()
                .Where(src => src.Name == name)
                .FirstOrDefault();

            if (result == null)
            {
                result = new IncidentLocation()
                {
                    Name = name
                };

                _Store.Save<IncidentLocation>(result);
            }

            return result;
        }

        public IncidentType GetOrCreateIncidentType(string name, IncidentTypeGroup group)
        {
            var result = GetQueryable<IncidentType>()
                .Where(src => src.Name == name && src.IncidentTypeGroup.Id == group.Id)
                .FirstOrDefault();

            if (result == null)
            {
                result = new IncidentType()
                {
                    Name = name,
                    IncidentTypeGroup = group
                };

                _Store.Save<IncidentType>(result);
            }

            return result;
        }

        public IncidentTypeGroup GetOrCreateIncidentTypeGroup(string name)
        {
            var result = GetQueryable<IncidentTypeGroup>()
                .Where(src => src.Name == name)
                .FirstOrDefault();

            if (result == null)
            {
                result = new IncidentTypeGroup()
                {
                    Name = name
                };

                _Store.Save<IncidentTypeGroup>(result);
            }

            return result;
        }

        public PsychotropicDrugName GetOrCreatePsychotropicDrugName(string name)
        {
            var result = GetQueryable<PsychotropicDrugName>()
                .Where(src => src.Name == name)
                .FirstOrDefault();

            if (result == null)
            {
                result = new PsychotropicDrugName()
                {
                    Name = name
                };

                _Store.Save<PsychotropicDrugName>(result);
            }

            return result;
        }

        public PsychotropicDrugType GetOrCreatePsychotropicDrugType(string name)
        {
            var result = GetQueryable<PsychotropicDrugType>()
                .Where(src => src.Name == name)
                .FirstOrDefault();

            if (result == null)
            {
                result = new PsychotropicDrugType()
                {
                    Name = name
                };

                _Store.Save<PsychotropicDrugType>(result);
            }

            return result;
        }

        public WoundClassification GetOrCreateWoundClassification(string name)
        {
            var result = GetQueryable<WoundClassification>()
            .Where(src => src.Name == name)
            .FirstOrDefault();

            if (result == null)
            {
                result = new WoundClassification()
                {
                    Name = name
                };

                _Store.Save<WoundClassification>(result);
            }

            return result;
        }


        public WoundType GetOrCreateWoundType(string name)
        {
            var result = GetQueryable<WoundType>()
            .Where(src => src.Name == name)
            .FirstOrDefault();

            if (result == null)
            {
                result = new WoundType()
                {
                    Name = name
                };

                _Store.Save<WoundType>(result);
            }

            return result;
        }

        public WoundStage GetOrCreateWoundStage(string name, int? rating)
        {
            var result = GetQueryable<WoundStage>()
            .Where(src => src.Name == name)
            .FirstOrDefault();

            if (result == null)
            {
                result = new WoundStage()
                {
                    Name = name,
                    Rating = rating
                };

                _Store.Save<WoundStage>(result);
            }

            return result;
        }

        public WoundSite GetOrCreateWoundSite(string name,int tlX, int tlY, int brX, int brY)
        {
            var result = GetQueryable<WoundSite>()
            .Where(src => src.Name == name)
            .FirstOrDefault();

            if (result == null)
            {
                result = new WoundSite()
                {
                    Name = name
                };

                _Store.Save<WoundSite>(result);
            }

            if (result.HasCoordinates(tlX, tlY, brX, brY) == false)
            {
                result.AddCoordinates(tlX, tlY, brX, brY);
                _Store.Save<WoundSite>(result);
            }


            return result;
        }

        public ComplaintType GetOrCreateComplaintType(string name)
        {
            var result = GetQueryable<ComplaintType>()
            .Where(src => src.Name == name)
            .FirstOrDefault();

            if (result == null)
            {
                result = new ComplaintType()
                {
                    Name = name
                };

                _Store.Save<ComplaintType>(result);
            }

            return result;
        }

        public void DeleteFacilityAverageType(FacilityAverageType src)
        {
            ((IQI.Intuition.Infrastructure.Persistence.Reporting.MongoDocumentStore)_Store)
                .Delete(src,src.Id);
        }

        public void AddFacilityAverageType(FacilityAverageType src)
        {
            _Store.Save(src);
        }


    }
}
