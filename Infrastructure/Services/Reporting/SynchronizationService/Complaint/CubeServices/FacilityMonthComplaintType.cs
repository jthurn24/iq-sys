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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Complaint.CubeServices
{
    public class FacilityMonthComplaintType : AbstractMonthlyFacilityCubeService
    {
        private Cubes.FacilityMonthComplaintType _Cube;
        private IEnumerable<Facts.Complaint> _Facts;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FacilityMonthComplaintType>()
                .Where(x => x.Facility.Id == changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthComplaintType();
                _Cube.Entries = new List<Cubes.FacilityMonthComplaintType.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            _Facts = GetQueryable<Facts.Complaint>()
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
            foreach (var complaintType in changes.ComplaintTypes)
            {

                var prevDataCount = _Facts
                    .Where(x =>
                        (x.Month.MonthOfYear == priorMonth.MonthOfYear && x.Month.Year == priorMonth.Year)
                        && x.ComplaintType.Name == complaintType.Name)
                        .Count();


                var prevRate = Domain.Calculations.Rate1000(prevDataCount, priorPatientDays);

                var currentData = _Facts
                .Where(x =>
                    (x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year)
                        && x.ComplaintType.Name == complaintType.Name);

                var currentDataCount = currentData.Count();

                Cubes.FacilityMonthComplaintType.Entry cube = _Cube.Entries
                    .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear &&
                        x.Month.Year == currentMonth.Year
                        && x.ComplaintType.Name == complaintType.Name)
                    .FirstOrDefault();

                if (cube == null)
                {
                    cube = new Cubes.FacilityMonthComplaintType.Entry();
                    cube.Month = currentMonth;
                    cube.ComplaintType = complaintType;
                    _Cube.Entries.Add(cube);
                }

                cube.Total = currentDataCount;
                cube.Rate = Domain.Calculations.Rate1000(cube.Total, currentPatientDays);
                cube.Change = 0 - (prevRate - cube.Rate);
                cube.Components = currentData.Select(x => x.Id);
                cube.ViewAction = "Complaints";
            }
        }

        protected override void Completed()
        {
            Save(_Cube);
            base.Completed();
        }



    }
}
