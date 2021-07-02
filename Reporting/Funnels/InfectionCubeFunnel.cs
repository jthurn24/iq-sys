using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubes = IQI.Intuition.Reporting.Models.Cubes;
using Virtual = IQI.Intuition.Reporting.Models.Cubes.Virtual;
using Facts = IQI.Intuition.Reporting.Models.Facts;
using RedArrow.Framework.Extensions.Common;
using SnyderIS.sCore.Extensions.Common;

namespace IQI.Intuition.Reporting.Funnels
{
    public static class InfectionCubeFunnel
    {
        public static IEnumerable<Virtual.FacilityGroupMonthInfectionType> CombineFacilityGroup(
            this IEnumerable<Cubes.FacilityMonthInfectionType.Entry> allStats,
            IEnumerable<Cubes.FacilityMonthCensus> allCensus
        )
        {
            var results = new List<Virtual.FacilityGroupMonthInfectionType>();

            foreach(var type in allStats.Select(x => x.InfectionType).Distinct())
            {
                int lastMonthPatientDays = 0;
                decimal lastMonthRate = 0;

                foreach (var month in allStats.Select(x => x.Month).Distinct().OrderBy(x => x.Year).ThenBy(x => x.MonthOfYear))
                {
                    var patientDays = allCensus.Where(x => x.Month == month).Sum(x => (int?)x.TotalPatientDays).ToIntSafely();

                    var entry = new Virtual.FacilityGroupMonthInfectionType();
                    entry.Month = month;
                    entry.InfectionType = type;
                    entry.Total = allStats.Where(x => x.Month == month && x.InfectionType == type).Sum(x => (int ?)x.Total).ToIntSafely();
                    entry.NonNosoTotal = allStats.Where(x => x.Month == month && x.InfectionType == type).Sum(x => (int?)x.NonNosoTotal).ToIntSafely();
                    entry.Rate = Domain.Calculations.Rate1000(entry.Total, patientDays);
                    entry.Change = Domain.Calculations.RateChange(lastMonthRate, entry.Rate);
                    results.Add(entry);

                    lastMonthPatientDays = patientDays;
                    lastMonthRate = entry.Rate;
                }
            }

            return results;
        }

    }
}
