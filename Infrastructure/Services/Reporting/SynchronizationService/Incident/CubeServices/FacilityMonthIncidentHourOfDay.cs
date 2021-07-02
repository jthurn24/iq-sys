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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Incident.CubeServices
{
    public class FacilityMonthIncidentHourOfDay : AbstractMonthlyFacilityCubeService
    {
        private Cubes.FacilityMonthIncidentHourOfDay _Cube;
        private IEnumerable<Facts.IncidentReport> _Facts;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FacilityMonthIncidentHourOfDay>()
                .Where(x => x.Facility.Id == changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthIncidentHourOfDay();
                _Cube.Entries = new List<Cubes.FacilityMonthIncidentHourOfDay.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            _Facts = GetQueryable<Facts.IncidentReport>()
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
            foreach (var incidentTypeGroup in changes.IncidentTypeGroups)
            {

                for (int hourOfDay = 0; hourOfDay <= 23; hourOfDay ++ )
                {

                    var prevDataCount = _Facts
                        .Where(x =>
                            (x.Month.MonthOfYear == priorMonth.MonthOfYear && x.Month.Year == priorMonth.Year)
                                && (x.OccurredOnHourOfDay == hourOfDay || (x.OccurredOnHourOfDay == null && x.DiscoveredOnHourOfDay == hourOfDay))
                                && x.IncidentTypeGroups.Contains(incidentTypeGroup)
                            )
                            .Count();


                    var prevRate = Domain.Calculations.Rate1000(prevDataCount, priorPatientDays);

                    var currentData = _Facts
                    .Where(x =>
                        (x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year)
                                && (x.OccurredOnHourOfDay == hourOfDay || (x.OccurredOnHourOfDay == null && x.DiscoveredOnHourOfDay == hourOfDay))
                                && x.IncidentTypeGroups.Contains(incidentTypeGroup)
                        );

                    var currentDataCount = currentData.Count();

                    Cubes.FacilityMonthIncidentHourOfDay.Entry cube = _Cube.Entries
                        .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year
                            && x.HourOfDay == hourOfDay
                            && x.IncidentTypeGroup.Name == incidentTypeGroup.Name)
                        .FirstOrDefault();

                    if (cube == null)
                    {
                        cube = new Cubes.FacilityMonthIncidentHourOfDay.Entry();
                        cube.Id = Guid.NewGuid();
                        _Cube.Entries.Add(cube);
                    }


                    cube.Month = currentMonth;
                    cube.HourOfDay = hourOfDay;
                    cube.IncidentTypeGroup = incidentTypeGroup;
                    cube.Total = currentDataCount;
                    cube.Rate = Domain.Calculations.Rate1000(cube.Total, currentPatientDays);
                    cube.Change = 0 - (prevRate - cube.Rate);
                    cube.ViewAction = "Incidents";
                    cube.Components = currentData.Select(x => x.Id);
                }
            }
        }


        protected override void Completed()
        {
            _Cube.Entries = _Cube.Entries.Where(x => x.Change != 0 || x.Total != 0 || x.Rate != 0).ToList();
            Save(_Cube);
            base.Completed();
        }
    }
}
