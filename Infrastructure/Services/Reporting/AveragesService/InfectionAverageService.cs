using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Persistence;
using Dimensions = IQI.Intuition.Reporting.Models.Dimensions;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using Facts = IQI.Intuition.Reporting.Models.Facts;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace IQI.Intuition.Infrastructure.Services.Reporting.AveragesService
{
    public class InfectionAverageService : IConsoleService
    {
        private IDimensionBuilderRepository _DimensionBuilderRepository;
        private IDimensionRepository _DimensionRepository;
        private ICubeBuilderRepository _CubeBuilderRepository;
        private IFactBuilderRepository _FactBuilderRespository;
        private ILog _Log;
        private int TotalPatientDays = 0;
        private int TotalFacilities = 0;
        private IDocumentStore _Store;

        public InfectionAverageService(
                IDimensionBuilderRepository dimensionBuilderRepository,
                IDimensionRepository dimensionRepository,
                ICubeBuilderRepository cubeBuilderRepository,
                IUnitOfWork unitOfWork,
                IFactBuilderRepository factBuilderRespository,
                ILog log,
                IDocumentStore store
            )
        {
            _DimensionBuilderRepository = dimensionBuilderRepository;
            _CubeBuilderRepository = cubeBuilderRepository;
            _FactBuilderRespository = factBuilderRespository;
            _DimensionRepository = dimensionRepository;
            _Log = log;
            _Store = store;
        }

        public void Run(string[] args)
        {
            var averages = GetQueryable<Dimensions.AverageType>();

            foreach (var average in averages)
            {
                System.Console.WriteLine("Calculating averages for {0} ", average);

                if (args.Length > 1)
                {
                    if (args[1] == "x")
                    {
                        for (int i = 36; i > 0; i--)
                        {
                            CalculateAverages(Convert.ToInt32(i), average);
                        }
                    }
                    else
                    {
                        CalculateAverages(Convert.ToInt32(args[1]), average);
                    }
                }
                else
                {
                    CalculateAverages(0, average);
                }
            }
        }

        private void CalculateAverages(int offset, Dimensions.AverageType averageType)
        {
            var month = DateTime.Today;

            if (offset > 0)
            {
                month = month.AddMonths(0 - offset);
            }

            System.Console.WriteLine("Calculating for {0} - {1} ", month.ToString("MM-yyyy"), averageType.Name);

            var currentMonth = _DimensionBuilderRepository.GetOrCreateMonth(month.Month, month.Year);

            var facilityGuids = GetQueryable<Dimensions.FacilityAverageType>()
                .Where(x => x.AverageType.Name == averageType.Name)
                .Select(x => x.Facility.Id)
                .Distinct();


            var infectionTypes = GetQueryable<Dimensions.InfectionType>();


            var averages = GetQueryable<Cubes.AverageTypeMonthInfectionType>()
                .Where(x => x.AverageType.Name == averageType.Name)
                .ToList();


            /* reset averages for the current month */
            this.TotalPatientDays = 0;
            this.TotalFacilities = 0;

            foreach (var currentAverage in averages.Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year))
            {
                currentAverage.Rate = 0;
                currentAverage.Total = 0;
            }


            /* Get Infection Totals and Total Patient Days */

            foreach (var facilityId in facilityGuids)
            {
                ApplyFacility(facilityId, averages, infectionTypes, currentMonth,averageType);
            }



            /* Calculate rates for current month  */

            foreach (var type in infectionTypes)
            {
                var average = GetQueryable<Cubes.AverageTypeMonthInfectionType>()
               .Where(x => x.AverageType.Name == averageType.Name)
               .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year)
               .Where(x => x.InfectionType.Name == type.Name)
               .OrderBy(x => x.Month.Year)
               .ThenBy(x => x.Month.MonthOfYear)
               .FirstOrDefault();

                if (average != null)
                {
                    average.Rate = Domain.Calculations.Rate1000(average.Total, this.TotalPatientDays);
                    average.TotalFacilities = this.TotalFacilities;
                    average.CensusPatientDays = this.TotalPatientDays;
                    average.CalculatedOn = DateTime.Now;
                    Save<Cubes.AverageTypeMonthInfectionType>(average);
                }

            }



        }

        private void ApplyFacility(Guid facilityId,
            IList<Cubes.AverageTypeMonthInfectionType> averages,
            IEnumerable<Dimensions.InfectionType> infectionTypes,
            Dimensions.Month month,
            Dimensions.AverageType averageType
            )
        {

            var dataEntry = GetQueryable<Cubes.FacilityMonthInfectionType>()
            .Where(x => x.Facility.Id == facilityId)
            .FirstOrDefault();

            if (dataEntry == null)
            {
                return;
            }

            var data = dataEntry
            .Entries
            .Where(x => x.Month.MonthOfYear == month.MonthOfYear && x.Month.Year == month.Year)
            .ToList();

            // first we need to qualify this facility. if they have not logged any facility aquired
            // infections they may not be using the system anymore (or correctly for that matter).
            // Also, if none of the patientcensus days have been entered we can't use the facility 
            // because rates were never calculated.

            if (data.Count() < 1 ||
                data.Max(x => x.Rate) == 0 ||
                data.Max(x => x.CensusPatientDays) < 1)
            {
                return;
            }

            // They qualify so aplly rates to averages
            this.TotalFacilities++;
            this.TotalPatientDays = this.TotalPatientDays + data.Max(x => x.CensusPatientDays);

                foreach (var type in infectionTypes)
                {

                    var total = data.Where(x => x.InfectionType.Name == type.Name)
                        .FirstOrDefault();

                    if (total != null)
                    {
                        var average = averages.Where(x => x.Month.MonthOfYear == month.MonthOfYear && x.Month.Year == month.Year)
                            .Where(x => x.InfectionType.Name == type.Name && x.AverageType.Name == averageType.Name)
                            .FirstOrDefault();

                        if (average == null)
                        {
                            average = new Cubes.AverageTypeMonthInfectionType();
                            average.InfectionType = type;
                            average.Month = month;
                            average.Total = total.Total;
                            average.AverageType = averageType;
                            averages.Add(average);
                            Insert<Cubes.AverageTypeMonthInfectionType>(average);
                        }
                        else
                        {
                            average.Total = average.Total + total.Total;
                            Save<Cubes.AverageTypeMonthInfectionType>(average);
                        }

                    }
            }
        }


        protected IQueryable<T> GetQueryable<T>()
        {
            return _Store.GetQueryable<T>();
        }


        protected void Insert<T>(T obj)
        {
            _Store.Insert<T>(obj);
        }

        protected void Save<T>(T obj)
        {
            _Store.Save<T>(obj);
        }
    }
}
