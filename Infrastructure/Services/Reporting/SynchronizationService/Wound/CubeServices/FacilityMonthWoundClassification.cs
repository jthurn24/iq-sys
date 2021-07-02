﻿using System;
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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Wound.CubeServices
{
    public class FacilityMonthWoundClassification : AbstractMonthlyFacilityCubeService
    {

        private Cubes.FacilityMonthWoundClassification _Cube;
        private IEnumerable<Facts.WoundReport> _Facts;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FacilityMonthWoundClassification>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthWoundClassification();
                _Cube.Entries = new List<Cubes.FacilityMonthWoundClassification.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }


            _Facts = GetQueryable<Facts.WoundReport>()
                .Where(x => x.Facility.Id == changes.Facility.Id
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
            var monthStartDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

            var priorMonthStartDate = monthStartDate.AddMonths(-1);
            var priorMonthEndDate = priorMonthStartDate.AddMonths(1).AddDays(-1);


            foreach (var type in changes.WoundTypes)
            {

                foreach (var classification in changes.WoundClassifications)
                {


                    var prevDataCount = _Facts
                        .Where(x =>
                            (x.ClosedOnDate == null || x.ClosedOnDate >= priorMonthStartDate || x.FirstNotedOnDate >= priorMonthStartDate) &&
                            ((x.ClosedOnDate == null && x.FirstNotedOnDate <= priorMonthEndDate) || x.ClosedOnDate <= priorMonthEndDate || x.FirstNotedOnDate <= priorMonthEndDate)
                            && x.Classification.Name == classification.Name && x.WoundType.Name == type.Name)
                            .Count();

                    var prevRate = Domain.Calculations.Rate1000(prevDataCount, priorPatientDays);

                    var currentData = _Facts
                        .Where(x =>
                            (x.ClosedOnDate == null || x.ClosedOnDate >= monthStartDate || x.FirstNotedOnDate >= monthStartDate) &&
                            ((x.ClosedOnDate == null && x.FirstNotedOnDate <= monthEndDate) || x.ClosedOnDate <= monthEndDate || x.FirstNotedOnDate <= monthEndDate)
                            && x.Classification.Name == classification.Name && x.WoundType.Name == type.Name);

                    var currentDataCount = currentData.Count();

                    Cubes.FacilityMonthWoundClassification.Entry cube = _Cube.Entries
                        .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year
                            && x.Classification.Name == classification.Name && x.WoundType.Name == type.Name)
                        .FirstOrDefault();

                    if (cube == null)
                    {
                        cube = new Cubes.FacilityMonthWoundClassification.Entry();
                        _Cube.Entries.Add(cube);

                    }

                    cube.Month = currentMonth;
                    cube.Classification = classification;
                    cube.WoundType = type;
                    cube.Total = currentDataCount;
                    cube.Rate = 0;
                    cube.PercentageOfPopulation = 0;
                    cube.ViewAction = "Wounds";
                    cube.Components = currentData.Select(x => x.Id);

                    decimal averagePatients = 0;

                    if (currentMonthCensus != null)
                    {
                        averagePatients = currentMonthCensus.Average;
                    }

                    if (cube.Total > 0 && currentPatientDays > 0)
                    {
                        cube.Rate = ((cube.Total / Convert.ToDecimal(currentPatientDays)) * 1000);
                    }

                    if (cube.Total > 0 && averagePatients > 0)
                    {
                        cube.PercentageOfPopulation = (cube.Total / averagePatients) * 100;
                    }


                    cube.Change = 0 - (prevRate - cube.Rate);

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