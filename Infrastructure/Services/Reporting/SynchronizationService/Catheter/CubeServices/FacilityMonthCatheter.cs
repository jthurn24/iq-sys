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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Catheter.CubeServices
{
    public class FacilityMonthCatheter : AbstractMonthlyFacilityCubeService
    {

        private Cubes.FacilityMonthCatheter _Cube;
        private IList<Facts.Catheter> _Facts;

        protected override void Init(DataDimensions changes)
        {
            _Log.Info(string.Format("Syncing cube FacilityMonthCatheter starting: {0} facility: {1}", changes.StartDate, changes.Facility.Name));

            _Cube = GetQueryable<Cubes.FacilityMonthCatheter>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();


            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthCatheter();
                _Cube.Entries = new List<Cubes.FacilityMonthCatheter.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            _Facts = GetQueryable<Facts.Catheter>()
                .Where(x =>
                x.Facility.Id == changes.Facility.Id
                && x.CatheterType != null
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
            foreach (var catheterType in GetQueryable<Dimensions.CatheterType>())
            {

                decimal prevRate = 0;

                var prevDataCount = _Facts
                    .Where(x => x.StartedDate <= priorDate.AddMonths(1).AddDays(-1)
                        && (x.DiscontinuedDate != null || x.DiscontinuedDate >= priorDate)
                        && x.CatheterType.Name == catheterType.Name)
                        .Count();

                if (priorPatientDays > 0 && prevDataCount > 0)
                {
                    prevRate = ((prevDataCount / Convert.ToDecimal(priorPatientDays)) * 1000);

                }

                var currentCatheters = _Facts
                    .Where(x =>
                        x.StartedDate <= currentDate.AddMonths(1).AddDays(-1) &&
                        (x.DiscontinuedDate.HasValue == false || x.DiscontinuedDate >= currentDate)
                        && x.CatheterType.Name == catheterType.Name);



                Cubes.FacilityMonthCatheter.Entry cube = _Cube.Entries
                    .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear
                        && x.Month.Year == currentMonth.Year
                        && x.CatheterType.Name == catheterType.Name)
                    .FirstOrDefault();

                if (cube == null)
                {
                    cube = new Cubes.FacilityMonthCatheter.Entry();
                    cube.Id = Guid.NewGuid();
                    _Cube.Entries.Add(cube);
                }

                cube.Components = currentCatheters.Select(x => x.Id).ToList();
                cube.ViewAction = "Catheters";
                cube.Month = currentMonth;
                cube.CatheterType = catheterType;
                cube.Total = currentCatheters.Count();
                cube.Rate = 0;

                if (cube.Total > 0 && currentPatientDays > 0)
                {
                    cube.Rate = ((cube.Total / Convert.ToDecimal(currentPatientDays)) * 1000);
                }

                cube.Change = 0 - (prevRate - cube.Rate);


                /* Calc out device days */

                int deviceDays = 0;
                var ddDate = currentDate;
                var endDDate = currentDate.AddMonths(1).AddDays(-1);

                if (endDDate >= DateTime.Today)
                {
                    endDDate = DateTime.Today;
                }

                while (ddDate <= endDDate)
                {
                    var ddCatheters = currentCatheters
                    .Where(x => x.StartedDate <= ddDate
                        && (x.DiscontinuedDate.HasValue == false || x.DiscontinuedDate >= ddDate)
                        && x.CatheterType.Name == catheterType.Name);

                    deviceDays = deviceDays + ddCatheters.Count();
                    ddDate = ddDate.AddDays(1);
                }

                cube.DeviceDays = deviceDays;

                if (deviceDays > 0 && currentPatientDays > 0)
                {
                    cube.UtilizationRatio = (decimal)deviceDays / (decimal)currentPatientDays;
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
