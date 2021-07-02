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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Infection.CubeServices
{
    public class FloorMonthInfectionType : AbstractMonthlyFacilityCubeService
    {

        private Cubes.FloorMonthInfectionType _Cube;
        private IEnumerable<Facts.InfectionVerification> _Noso_Facts;
        private IEnumerable<Facts.InfectionVerification> _NonNoso_Facts;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FloorMonthInfectionType>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FloorMonthInfectionType();
                _Cube.Entries = new List<Cubes.FloorMonthInfectionType.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            _Noso_Facts = GetQueryable<Facts.InfectionVerification>()
                    .Where(x =>
                        x.InfectionClassification.IsQualified && x.InfectionClassification.IsNosocomial
                        && x.Facility.Id == changes.Facility.Id
                        && (x.Deleted == null || x.Deleted == false)).ToList();

            _NonNoso_Facts = GetQueryable<Facts.InfectionVerification>()
                        .Where(x =>
                        x.InfectionClassification.IsQualified && x.InfectionClassification.IsNosocomial == false
                        && x.Facility.Id == changes.Facility.Id
                        && (x.Deleted == null || x.Deleted == false)).ToList();
        }

        protected override void ProcessDay(DataDimensions changes,
            DateTime currentDate,
            DateTime priorDate,
            Dimensions.Month currentMonth,
            Dimensions.Month priorMonth,
            Cubes.FacilityMonthCensus currentMonthCensus,
            Cubes.FacilityMonthCensus priorMonthCensus,
            int currentPatientDays,
            int priorPatientDays)
        {

                foreach (var infectionType in changes.InfectionTypes)
                {
                    foreach (var floor in changes.Floors)
                    {


                        var prevDataCount = _Noso_Facts
                            .Where(x =>
                                (x.NotedOnMonth.MonthOfYear == priorMonth.MonthOfYear && x.NotedOnMonth.Year == priorMonth.Year)
                                && x.InfectionType.Name == infectionType.Name
                                && x.Floor.Id == floor.Id
                                )
                                .Count();


                        var prevRate = Domain.Calculations.Rate1000(prevDataCount, priorPatientDays);

                        var currentData = _Noso_Facts
                        .Where(x =>
                            (x.NotedOnMonth.MonthOfYear == currentMonth.MonthOfYear && x.NotedOnMonth.Year == currentMonth.Year)
                                && x.InfectionType.Name == infectionType.Name
                                && x.Floor.Id == floor.Id
                            );

                        var currentDataCount = currentData.Count();

                        Cubes.FloorMonthInfectionType.Entry cube = _Cube.Entries
                            .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year
                                && x.InfectionType.Name == infectionType.Name
                                && x.Floor.Id == floor.Id)
                            .FirstOrDefault();

                        if (cube == null)
                        {
                            cube = new Cubes.FloorMonthInfectionType.Entry();
                            cube.Id = Guid.NewGuid();
                            _Cube.Entries.Add(cube);
                        }

                        cube.Components = currentData.Select(x => x.Id).ToList();
                        cube.ViewAction = "Infections";
                        cube.Month = currentMonth;
                        cube.InfectionType = infectionType;
                        cube.Floor = floor;
                        cube.Total = currentDataCount;
                        cube.Rate = Domain.Calculations.Rate1000(cube.Total, currentPatientDays);
                        

                        //cube.NonNosoTotal = _NonNoso_Facts.Where(
                        //    x => x.InfectionSite.Name == infectionSite.Name
                        //    && x.NotedOnMonth.MonthOfYear == priorMonth.MonthOfYear
                        //    && x.NotedOnMonth.Year == priorMonth.Year).Count();

                        //cube.Change = 0 - (prevRate - cube.Rate);

                    }
            }
        }


        protected override void Completed()
        {
            Save(_Cube);
            base.Completed();
        }

    }
}
