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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.SymptomaticCUATI.CubeServices
{
    public class FacilityMonthSCAUTI : AbstractMonthlyFacilityCubeService
    {

        private Cubes.FacilityMonthSCAUTI _Cube;
        private IEnumerable<Cubes.FacilityMonthCatheter.Entry> _Catheters;
        private IEnumerable<Facts.InfectionVerification> _Infections;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FacilityMonthSCAUTI>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthSCAUTI();
                _Cube.Entries = new List<Cubes.FacilityMonthSCAUTI.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            var catheterCube = GetQueryable<Cubes.FacilityMonthCatheter>()
                    .Where(x => x.Facility.Id == changes.Facility.Id)
                    .FirstOrDefault();

            if (catheterCube == null)
            {
                _Catheters = new List<Cubes.FacilityMonthCatheter.Entry>();
            }
            else
            {
                _Catheters = catheterCube.Entries.Where(x => x.CatheterType.Name == "Urethral");
            }


            _Infections = GetQueryable<Facts.InfectionVerification>()
                        .Where(x =>
                        x.InfectionClassification.IsQualified && x.InfectionClassification.IsNosocomial
                        && x.Facility.Id == changes.Facility.Id
                        && x.SupportingDetail == "Indwelling"
                        && x.InfectionSite.Name == "UTI with catheter"
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

            /* Get current device days stats */
            var currentDeviceData = _Catheters
                .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year)
                .FirstOrDefault();

            int deviceDays = 0;

            if (currentDeviceData != null)
            {
                deviceDays = currentDeviceData.DeviceDays;
            }

            /* Get prior months device days */

            var priorDeviceData = _Catheters
                .Where(x => x.Month.MonthOfYear == priorMonth.MonthOfYear && x.Month.Year == priorMonth.Year)
                .FirstOrDefault();

            int priorDeviceDays = 0;

            if (priorDeviceData != null)
            {
                priorDeviceDays = priorDeviceData.DeviceDays;
            }

            /* Calc rates */

            var prevDataCount = _Infections
                .Where(x =>
                   x.NotedOnMonth.MonthOfYear == priorMonth.MonthOfYear && x.NotedOnMonth.Year == priorMonth.Year)
                    .Count();

            decimal prevRate = 0;
            prevRate = Domain.Calculations.Rate1000(prevDataCount, priorDeviceDays);


            var curData = _Infections
                .Where(x => x.NotedOnMonth.MonthOfYear == currentMonth.MonthOfYear && x.NotedOnMonth.Year == currentMonth.Year);

            var curDataCount = curData.Count();

            decimal curRate = 0;
            curRate = Domain.Calculations.Rate1000(curDataCount, deviceDays);


            /* UPdate cube */

            Cubes.FacilityMonthSCAUTI.Entry cube = _Cube.Entries
                .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year)
                .FirstOrDefault();

            if (cube == null)
            {
                cube = new Cubes.FacilityMonthSCAUTI.Entry();
                cube.Month = currentMonth;
                _Cube.Entries.Add(cube);
            }

            cube.DeviceDays = deviceDays;
            cube.Total = curDataCount;
            cube.Rate = curRate;
            cube.Change = Domain.Calculations.RateChange(prevRate, curRate);
            cube.Components = curData.Select(x => x.Id);
            cube.ViewAction = "Infections";
        }

        protected override void Completed()
        {
            Save(_Cube);
            base.Completed();
        }
    }
}
