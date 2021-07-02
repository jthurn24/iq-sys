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

namespace IQI.Intuition.Infrastructure.Services.Reporting.SynchronizationService.Incident.CubeServices
{
    public class FacilityMonthIncidentInjuryLevel : AbstractMonthlyFacilityCubeService
    {

        private Cubes.FacilityMonthIncidentInjuryLevel _Cube;
        private IEnumerable<Facts.IncidentReport> _Facts;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.FacilityMonthIncidentInjuryLevel>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.FacilityMonthIncidentInjuryLevel();
                _Cube.Entries = new List<Cubes.FacilityMonthIncidentInjuryLevel.Entry>();
                _Cube.Facility = changes.Facility;
                _Cube.Account = changes.Facility.Account;
            }

            _Facts = GetQueryable<Facts.IncidentReport>()
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
            foreach (var incidentTypeGroup in changes.IncidentTypeGroups)
            {

                foreach (var level in changes.IncidentInjuryLevels)
                {

                    var prevDataCount = _Facts
                        .Where(x =>
                            (x.Month.MonthOfYear == priorMonth.MonthOfYear && x.Month.Year == priorMonth.Year)
                            &&
                                x.IncidentInjuryLevel.Name == level.Name
                                && x.IncidentTypeGroups.Contains(incidentTypeGroup)
                            )
                            .Count();


                    var prevRate = Domain.Calculations.Rate1000(prevDataCount, priorPatientDays);

                    var currentData = _Facts
                    .Where(x =>
                        (x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year)
                         &&
                                x.IncidentInjuryLevel.Name == level.Name
                                && x.IncidentTypeGroups.Contains(incidentTypeGroup)
                        );

                    var currentDataCount = currentData.Count();

                    Cubes.FacilityMonthIncidentInjuryLevel.Entry cube = _Cube.Entries
                        .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year
                            && x.IncidentInjuryLevel.Name == level.Name
                            && x.IncidentTypeGroup.Name == incidentTypeGroup.Name)
                        .FirstOrDefault();

                    if (cube == null)
                    {
                        cube = new Cubes.FacilityMonthIncidentInjuryLevel.Entry();
                        cube.Id = Guid.NewGuid();
                        _Cube.Entries.Add(cube);
                    }


                    cube.Month = currentMonth;
                    cube.IncidentInjuryLevel = level;
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
            Save(_Cube);
            base.Completed();
        }
    }
}