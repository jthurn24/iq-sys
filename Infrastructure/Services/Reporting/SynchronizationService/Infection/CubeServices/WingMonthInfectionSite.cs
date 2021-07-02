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
    public class WingMonthInfectionSite : AbstractMonthlyFacilityCubeService
    {
        private Cubes.WingMonthInfectionSite _Cube;
        private IEnumerable<Facts.InfectionVerification> _Noso_Facts;
        private IEnumerable<Facts.InfectionVerification> _NonNoso_Facts;

        protected override void Init(DataDimensions changes)
        {

            _Cube = GetQueryable<Cubes.WingMonthInfectionSite>()
                .Where(x => x.Facility.Id== changes.Facility.Id)
                .FirstOrDefault();

            if (_Cube == null)
            {
                _Cube = new Cubes.WingMonthInfectionSite();
                _Cube.Account = changes.Facility.Account;
                _Cube.Facility = changes.Facility;
                _Cube.InfectionSiteEntries = new List<Cubes.WingMonthInfectionSite.InfectionSiteEntry>();
                Save <Cubes.WingMonthInfectionSite>(_Cube);
            }

            
          
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

            foreach (var infectionSite in changes.InfectionSites)
            {
                var infectionSiteEntry = _Cube.InfectionSiteEntries
                    .Where(x => x.InfectionSite.Name == infectionSite.Name &&
                    x.InfectionType.Name == infectionSite.InfectionType.Name)
               .FirstOrDefault();

                if(infectionSiteEntry == null)
                {
                    infectionSiteEntry = new Cubes.WingMonthInfectionSite.InfectionSiteEntry();
                    infectionSiteEntry.InfectionType = infectionSite.InfectionType;
                    infectionSiteEntry.InfectionSite = infectionSite;
                    infectionSiteEntry.WingEntries = new List<Cubes.WingMonthInfectionSite.WingEntry>();
                    _Cube.InfectionSiteEntries.Add(infectionSiteEntry);
                }

                foreach (var wing in changes.Wings)
                {

                    var wingEntry = infectionSiteEntry.WingEntries.Where(x => x.Wing.Id == wing.Id).FirstOrDefault();

                    if (wingEntry == null)
                    {
                        wingEntry = new Cubes.WingMonthInfectionSite.WingEntry();
                        wingEntry.Wing = wing;
                        wingEntry.Entries = new List<Cubes.WingMonthInfectionSite.Entry>();
                        infectionSiteEntry.WingEntries.Add(wingEntry);
                    }

                    var prevDataCount = _Noso_Facts
                        .Where(x =>
                            (x.NotedOnMonth.MonthOfYear == priorMonth.MonthOfYear && x.NotedOnMonth.Year == priorMonth.Year)
                            && x.InfectionSite.Name == infectionSite.Name
                            && x.Wing.Id == wing.Id
                            )
                            .Count();


                    var prevRate = Domain.Calculations.Rate1000(prevDataCount, priorPatientDays);

                    var currentData = _Noso_Facts
                    .Where(x =>
                        (x.NotedOnMonth.MonthOfYear == currentMonth.MonthOfYear && x.NotedOnMonth.Year == currentMonth.Year)
                            && x.InfectionSite.Name == infectionSite.Name
                            && x.Wing.Id == wing.Id
                        );

                    var currentDataCount = currentData.Count();


                    Cubes.WingMonthInfectionSite.Entry entry = wingEntry.Entries
                        .Where(x => x.Month.MonthOfYear == currentMonth.MonthOfYear && x.Month.Year == currentMonth.Year)
                        .FirstOrDefault();

                    if (entry == null)
                    {
                        entry = new Cubes.WingMonthInfectionSite.Entry();
                        entry.Id = Guid.NewGuid();
                        wingEntry.Entries.Add(entry);
                    }

                    entry.Month = currentMonth;
                    entry.Total = currentDataCount;
                    entry.Rate = Domain.Calculations.Rate1000(entry.Total, currentPatientDays);
                    entry.Components = currentData.Select(x => x.Id);
                    entry.ViewAction = "Infections";


                    entry.Change = 0 - (prevRate - entry.Rate);

                    // Cleanup unneeded records
                    wingEntry.Entries = wingEntry.Entries.Where(x => x.Change != 0 || x.Rate != 0 || x.Total != 0).ToList();

                }
            }

            
        }


        protected override void Completed()
        {
            Save<Cubes.WingMonthInfectionSite>(_Cube);
            base.Completed();
        }

    }


}
