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
using RedArrow.Framework.Utilities;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Infection.CubeServices
{
    public class FacilityMonthInfectionType : AbstractMonthlyFacilityCubeService
    {

        private Cubes.FacilityMonthInfectionType _Cube;
        private IEnumerable<Facts.InfectionVerification> _Noso_Facts;
        private IEnumerable<Facts.InfectionVerification> _NonNoso_Facts;


        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FacilityMonthInfectionType>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthInfectionType();
                _Cube.Entries = new List<Cubes.FacilityMonthInfectionType.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            _Cube.Facility = changes.Facility;
            _Cube.Account = changes.Facility.Account;
            Save<Cubes.FacilityMonthInfectionType>(_Cube);

            _Noso_Facts = GetQueryable<Facts.InfectionVerification>()
                    .Where(x =>
                        x.InfectionClassification.IsQualified && x.InfectionClassification.IsNosocomial
                        && x.Facility.Id == changes.Facility.Id
                        && (x.Deleted == null || x.Deleted == false))
                        .ToList();

            _NonNoso_Facts = GetQueryable<Facts.InfectionVerification>()
                        .Where(x =>
                        x.InfectionClassification.IsQualified && x.InfectionClassification.IsNosocomial == false
                        && x.Facility.Id == changes.Facility.Id
                        && (x.Deleted == null || x.Deleted == false))
                        .ToList();
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


                var prevDataCount = _Noso_Facts
                    .Where(x =>
                        (x.NotedOnMonth.MonthOfYear == priorMonth.MonthOfYear && x.NotedOnMonth.Year == priorMonth.Year)
                        && x.InfectionType.Name == infectionType.Name
                        )
                        .Count();


                var prevRate = Domain.Calculations.Rate1000(prevDataCount, priorPatientDays);

                var currentData = _Noso_Facts
                .Where(x =>
                    (x.NotedOnMonth.MonthOfYear == currentMonth.MonthOfYear && x.NotedOnMonth.Year == currentMonth.Year)
                        && x.InfectionType.Name == infectionType.Name
                    );

                var currentDataCount = currentData.Count();

                Cubes.FacilityMonthInfectionType.Entry entry = _Cube.Entries
                    .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year
                        && x.InfectionType.Name == infectionType.Name)
                    .FirstOrDefault();

                if (entry == null)
                {
                    entry = new Cubes.FacilityMonthInfectionType.Entry();
                    entry.Id = Guid.NewGuid();
                    _Cube.Entries.Add(entry);
                }

                entry.Month = currentMonth;
                entry.InfectionType = infectionType;
                entry.Total = currentDataCount;
                entry.Rate = Domain.Calculations.Rate1000(entry.Total, currentPatientDays);
                entry.Components = currentData.Select(x => x.Id).ToList();
                entry.ViewAction = "Infections";
                entry.CensusPatientDays = currentPatientDays;

                entry.NonNosoTotal = _NonNoso_Facts.Where(
                    x => x.InfectionType.Name ==  infectionType.Name
                    && x.NotedOnMonth.MonthOfYear == currentMonth.MonthOfYear
                    && x.NotedOnMonth.Year == currentMonth.Year).Count();

                entry.Change = 0 - (prevRate - entry.Rate);

            }
        }

        protected override void Completed()
        {
            Save<Cubes.FacilityMonthInfectionType>(_Cube);
            base.Completed();
        }


    }
}
