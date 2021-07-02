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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Wound.CubeServices
{
    public class WingMonthWoundType : AbstractMonthlyFacilityCubeService
    {

        private Cubes.WingMonthWoundType _Cube;
        private IEnumerable<Facts.WoundReport> _Facts;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.WingMonthWoundType>()
                .Where(x => x.Facility.Id == changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.WingMonthWoundType();
                _Cube.Entries = new List<Cubes.WingMonthWoundType.Entry>();
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

            foreach (var woundType in changes.WoundTypes)
            {

                foreach (var wing in changes.Wings)
                {


                    var typeReports = _Facts
                        .Where(x => x.WoundType.Id == woundType.Id);



                    decimal prevRate = 0;

                    var prevDataCount = typeReports
                        .Where(x => x.Assessments
                            .Where(xx => xx.Wing.Id == wing.Id && xx.Month.Id == priorMonth.Id).Count() > 0)
                        .Select(x => x.Id).Distinct().Count();


                    var currentData = typeReports
                        .Where(x => x.Assessments
                            .Where(xx => xx.Wing.Id == wing.Id && xx.Month.Id == currentMonth.Id).Count() > 0)
                        .Select(x => x.Id).Distinct();


                    var currentDataCount = currentData.Count();


                    Cubes.WingMonthWoundType.Entry cube = _Cube.Entries
                        .Where(x => x.Month.Id == currentMonth.Id &&
                            x.WoundType.Id == woundType.Id && x.Wing.Id == wing.Id)
                        .FirstOrDefault();

                    if (cube == null)
                    {
                        cube = new Cubes.WingMonthWoundType.Entry();
                        _Cube.Entries.Add(cube);
                    }

                    cube.Month = currentMonth;
                    cube.Wing = wing;
                    cube.WoundType = woundType;
                    cube.Total = currentDataCount;
                    cube.Rate = 0;
                    cube.ViewAction = "Wounds";
                    cube.Components = currentData;

                    if (cube.Total > 0 && currentPatientDays > 0)
                    {
                        cube.Rate = ((cube.Total / Convert.ToDecimal(currentPatientDays)) * 1000);
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
