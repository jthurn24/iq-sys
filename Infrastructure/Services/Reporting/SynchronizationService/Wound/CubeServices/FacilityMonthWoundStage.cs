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
using RedArrow.Framework.Utilities;
using SnyderIS.sCore.Persistence;
using IQI.Intuition.Reporting.Containers;

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Wound.CubeServices
{
    public class FacilityMonthWoundStage : AbstractMonthlyFacilityCubeService
    {

        private Cubes.FacilityMonthWoundStage _Cube;
        private IEnumerable<Facts.WoundReport> _Facts;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FacilityMonthWoundStage>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthWoundStage();
                _Cube.Entries = new List<Cubes.FacilityMonthWoundStage.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            _Facts = GetQueryable<Facts.WoundReport>()
                .Where(x =>  x.Facility.Id == changes.Facility.Id
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

            var allStages = GetQueryable<Dimensions.WoundStage>();

            var stageCalcService = new IQI.Intuition.Infrastructure.Services.BusinessLogic.Wound.ReportingStageCalulator();


            foreach (var type in changes.WoundTypes)
            {

                foreach (var stage in allStages)
                {

                    var allreports = _Facts
                        .Where(x => x.WoundType.Name == type.Name);

                    var allAssessments = allreports.SelectMany(x => x.Assessments);

                    decimal prevRate = 0;

                    var prevDataCount = stageCalcService
                        .AnalyzeStages(allreports, allStages, priorMonthStartDate, priorMonthEndDate)
                        .Where(x => x.MaxStage.Name == stage.Name)
                        .Count();

                    if (priorPatientDays > 0 && prevDataCount > 0)
                    {
                        prevRate = ((prevDataCount / Convert.ToDecimal(priorPatientDays)) * 1000);

                    }

                    var currentData = stageCalcService
                        .AnalyzeStages(allreports, allStages, monthStartDate, monthEndDate)
                        .Where(x => x.MaxStage.Name == stage.Name) ;

                    var currentDataCount = currentData.Count();

                    Cubes.FacilityMonthWoundStage.Entry cube = _Cube.Entries
                        .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear
                            && x.Month.Year == currentMonth.Year
                            && x.Stage.Name == stage.Name
                            && x.WoundType.Name == type.Name)
                        .FirstOrDefault();

                    if (cube == null)
                    {
                        cube = new Cubes.FacilityMonthWoundStage.Entry();
                        _Cube.Entries.Add(cube);
                    }

                    cube.Month = currentMonth;
                    cube.Stage = stage;
                    cube.WoundType = type;
                    cube.Total = currentDataCount;
                    cube.Rate = 0;
                    cube.PercentageOfPopulation = 0;
                    cube.CensusPatientDays = currentPatientDays;
                    cube.ViewAction = "Wounds";
                    cube.Components = currentData.Select(x => x.Report.Id);
                    cube.Id = GuidHelper.NewGuid();

                    if (cube.Total > 0 && currentPatientDays > 0)
                    {
                        cube.Rate = ((cube.Total / Convert.ToDecimal(currentPatientDays)) * 1000);
                    }

                    decimal averagePatients = 0;

                    if (currentMonthCensus != null)
                    {
                        averagePatients = currentMonthCensus.Average;
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